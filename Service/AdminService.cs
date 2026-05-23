using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.MainRepositories.Repository;
using FreightManagement.Models;
using FreightManagement.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace FreightManagement.Service
{
    public class AdminService
    {
        private readonly IDonHangsRepository _OrderRepo;
        private readonly IUsersRepository _UsersRepo;
        private readonly IKhoHangsRepository _WareRepo;
        private readonly IThongKeDoanhThusRepository _TKRepo;

        public AdminService(IDonHangsRepository OrderRepo, IUsersRepository UsersRepo,
                            IKhoHangsRepository WareRepo, IThongKeDoanhThusRepository TKRepo)
        {
            _OrderRepo = OrderRepo;
            _UsersRepo = UsersRepo;
            _WareRepo = WareRepo;
            _TKRepo = TKRepo;
        }

        // ── Dashboard ────────────────────────────────────────────────────────
        public async Task<int> GetDonHangsCount()
        {
            return await _OrderRepo.GetDonHangsCount();
        }

        public async Task<int> GetDonHangsCountByTrangThai(string trangthai)
        {
            return await _OrderRepo.GetFullOrdersCountByTrangThai(trangthai);
        }

        public async Task<int> GetUsersCountByRoleId(int roleId)
        {
            return await _UsersRepo.GetUsersCountByRoleId(roleId);
        }

        public async Task<int> AdminGetTongKho()
        {
            return await _WareRepo.AdminGetTongKho();
        }

        public async Task<int> AdminGetSum()
        {
            return await _TKRepo.AdminGetSum();
        }

        public async Task<List<DonHang>> GetRecentDonHangs()
        {
            return await _OrderRepo.GetRecentDonHangs();
        }

        // ── Users ────────────────────────────────────────────────────────────
        public async Task<List<Users>> GetUsersList()
        {
            return await _UsersRepo.GetUsersList();
        }

        public async Task<(bool IsValidUser, string message)> ToggleAccountService(int id)
        {
            var user = await _UsersRepo.GetUserById(id);
            if (user != null && user.RoleId != 1)
            {
                user.TrangThai = user.TrangThai == "HoatDong" ? "BiKhoa" : "HoatDong";
                await _UsersRepo.UpdateUser(user);
                if (user.TrangThai == "HoatDong")
                    return (true, $"Đã mở khóa tài khoản {user.HoTen}.");
                else
                    return (true, $"Đã khóa tài khoản {user.HoTen}.");
            }
            return (false, "");
        }

        // ── YC3: Thêm nhân viên ─────────────────────────────────────────────
        public async Task<(bool success, string message)> AddStaffService(AddStaffDTO dto)
        {
            if (dto.RoleId != 3 && dto.RoleId != 4)
                return (false, "Chức vụ không hợp lệ. Chỉ được thêm Quản lý kho hoặc Tài xế.");

            if (await _UsersRepo.CheckExistUserByEmail(dto.Email))
                return (false, "Email này đã được sử dụng.");

            // Kiểm tra trùng SĐT tài xế
            if (!string.IsNullOrWhiteSpace(dto.SoDienThoai) && dto.RoleId == 4)
            {
                if (await _UsersRepo.CheckExistDriverBySoDienThoai(dto.SoDienThoai.Trim()))
                    return (false, "Số điện thoại này đã được sử dụng bởi tài xế khác.");
            }

            // Kiểm tra trùng CCCD tài xế
            if (dto.RoleId == 4)
            {
                if (string.IsNullOrWhiteSpace(dto.CCCD))
                    return (false, "Số CCCD là bắt buộc đối với Tài xế.");
                if (await _UsersRepo.CheckExistDriverByCCCD(dto.CCCD.Trim()))
                    return (false, "Số CCCD này đã được sử dụng bởi tài xế khác.");
            }

            var user = new Users
            {
                HoTen = dto.HoTen.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = HashPassword(dto.Password),
                SoDienThoai = dto.SoDienThoai?.Trim(),
                DiaChi = dto.DiaChi?.Trim(),
                CCCD = dto.RoleId == 4 ? dto.CCCD?.Trim() : null,
                RoleId = dto.RoleId,
                TrangThai = "HoatDong"
            };

            await _UsersRepo.AddUser(user);
            var roleName = dto.RoleId == 3 ? "Quản lý kho" : "Tài xế";
            return (true, $"Đã thêm {roleName} {user.HoTen} thành công!");
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        // ── YC5: Orders filter + phân trang ─────────────────────────────────
        public async Task<(List<DonHang> items, int totalCount)> GetFullOrdersFiltered(
            string? trangThai, string? tuNgay, string? denNgay,
            string? tuKhoa, int page, int pageSize = 20)
        {
            return await _OrderRepo.GetFullOrdersFiltered(trangThai, tuNgay, denNgay, tuKhoa, page, pageSize);
        }

        // ── Warehouses ───────────────────────────────────────────────────────
        public async Task<List<KhoHang>> AdminGetKhoHangToWarehouses()
        {
            return await _WareRepo.AdminGetKhoHangToWarehouses();
        }

        // ── GÁN KHO CHO QUẢN LÝ KHO ─────────────────────────────────────────
        public async Task<List<Users>> GetAllQuanLyKho()
        {
            return await _WareRepo.GetAllQuanLyKho();
        }

        public async Task<KhoHang> GetKhoHangById(int maKho)
        {
            return await _WareRepo.GetKhoHangById(maKho);
        }

        public async Task<(bool success, string message)> AssignKhoToQuanLyKho(int maKho, int maQLK)
        {
            var kho = await _WareRepo.GetKhoHangById(maKho);
            if (kho == null)
                return (false, "Không tìm thấy kho hàng.");

            var qlk = await _UsersRepo.GetUserById(maQLK);
            if (qlk == null || qlk.RoleId != 3)
                return (false, "Quản lý kho không tồn tại hoặc không hợp lệ.");

            kho.MaQLK = maQLK;
            await _WareRepo.UpdateKhoHang(kho);
            return (true, $"Đã gán kho {kho.TenKho} cho {qlk.HoTen} thành công!");
        }

        // ── Revenue ──────────────────────────────────────────────────────────
        public async Task<List<ThongKeDoanhThu>> AdminGetToRevenue()
        {
            return await _TKRepo.AdminGetToRevenue();
        }

        // ── Cập nhật CCCD tài xế (chỉ Admin) ────────────────────────────────
        public async Task<(bool success, string message)> UpdateDriverCCCD(int driverId, string cccd)
        {
            var driver = await _UsersRepo.GetUserById(driverId);
            if (driver == null || driver.RoleId != 4)
                return (false, "Không tìm thấy tài xế.");

            if (string.IsNullOrWhiteSpace(cccd))
                return (false, "Số CCCD không được để trống.");

            if (await _UsersRepo.CheckExistDriverByCCCD(cccd.Trim(), driverId))
                return (false, "Số CCCD này đã được sử dụng bởi tài xế khác.");

            driver.CCCD = cccd.Trim();
            await _UsersRepo.UpdateUser(driver);
            return (true, $"Đã cập nhật CCCD cho tài xế {driver.HoTen}.");
        }
    }
}