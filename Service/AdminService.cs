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
            return await _OrderRepo.GetDonHangsCountByTrangThai(trangthai);
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
        public async Task<List<User>> GetUsersList()
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
                return user.TrangThai == "HoatDong"
                    ? (true, $"Đã mở khóa tài khoản {user.HoTen}.")
                    : (true, $"Đã khóa tài khoản {user.HoTen}.");
            }
            return (false, "");
        }

        // ── YC3: Thêm nhân viên (TaiXe=4 / QuanLyKho=3) ────────────────────
        public async Task<(bool success, string message)> AddStaffService(AddStaffDTO dto)
        {
            if (dto.RoleId != 3 && dto.RoleId != 4)
                return (false, "Chức vụ không hợp lệ. Chỉ được thêm Quản lý kho hoặc Tài xế.");

            if (await _UsersRepo.CheckExistUserByEmail(dto.Email))
                return (false, "Email này đã được sử dụng.");

            var user = new User
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

        // ── YC5: Orders có filter + phân trang ──────────────────────────────
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

        // ── Revenue ──────────────────────────────────────────────────────────
        public async Task<List<ThongKeDoanhThu>> AdminGetToRevenue()
        {
            return await _TKRepo.AdminGetToRevenue();
        }
    }
}