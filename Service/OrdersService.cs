using FreightManagement.DTOs;
using FreightManagement.MainRepositories;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.Models;

namespace FreightManagement.Service
{
    public class OrdersService
    {
        private readonly IDonHangsRepository _OrdersRepo;
        private readonly IUsersRepository _UsersRepo;
        private readonly ILichSuTrangThaisRepository _hisRepo;

        public OrdersService(IDonHangsRepository OrdersRepo, IUsersRepository UsersRepo, ILichSuTrangThaisRepository hisRepo)
        {
            _OrdersRepo = OrdersRepo;
            _UsersRepo = UsersRepo;
            _hisRepo = hisRepo;
        }

        public async Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid)
            => await _OrdersRepo.OrdersGetDonHangsByMaKHAndDescending(uid);

        public async Task<Users> GetUserById(int UserId)
            => await _UsersRepo.GetUserById(UserId);

        public async Task<(bool success, string message)> CreateOrderService(int userId, CreateOrderDTO obj)
        {
            // Kiểm tra SĐT người nhận đã được dùng trong đơn nào chưa
            if (!string.IsNullOrWhiteSpace(obj.SdtNguoiNhan))
            {
                if (await _OrdersRepo.CheckExistSdtNguoiNhan(obj.SdtNguoiNhan.Trim()))
                    return (false, "Số điện thoại người nhận này đã được sử dụng trong một đơn hàng khác.");
            }

            int donGia = obj.LoaiHang switch
            {
                "HangHoa" => 25000,
                "ThuTu" => 20000,
                _ => 30000
            };
            var user = await _UsersRepo.GetUserById(userId);
            var order = new DonHang
            {
                MaKH = userId,
                DiaChiGui = user?.DiaChi ?? "Chưa cập nhật",
                TenNguoiNhan = obj.TenNguoiNhan!,
                SdtNguoiNhan = obj.SdtNguoiNhan!,
                DiaChiNhan = $"{obj.DiaChiNhan}, {obj.TinhNhan}",
                MoTaHang = obj.MoTaHang,
                SoLuong = obj.SoLuong,
                DonGia = donGia,
                ChiPhi = donGia * obj.SoLuong,
                TrangThai = "Đang xử lý"
            };
            await _OrdersRepo.OrdersAddDonHangs(order);
            await _hisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = order.MaDon,
                TrangThai = "Đang xử lý",
                GhiChu = "Đơn hàng đã được đặt thành công",
                CapNhatBoi = userId,
            });
            return (true, $"Đặt đơn hàng #DH{order.MaDon:D5} thành công!");
        }

        public async Task<(bool IsValidOrder, DonHang order)> DetailOrderService(int id, int userId)
        {
            var dh = await _OrdersRepo.OrdersGetDonHangsIncludeByMaDonAndMaKH(id, userId);
            if (dh == null) return (false, null!);
            return (true, dh);
        }

        // ── YC2a FIX: thêm UpdateOrder sau khi set TrangThai ──────
        public async Task<(bool IsValidOrder, string message)> CancelService(int id, int userId)
        {
            var order = await _OrdersRepo.OrdersGetDonHangsByMaDonAndMaKH(id, userId);
            if (order == null || order.TrangThai != "Đang xử lý")
                return (false, "Không thể hủy đơn hàng này.");

            order.TrangThai = "Đã hủy";
            await _OrdersRepo.UpdateOrder(order);   // ← FIX: lưu vào DB

            await _hisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = id,
                TrangThai = "Đã hủy",
                GhiChu = "Khách hàng hủy đơn",
                CapNhatBoi = userId,
            });
            return (true, "Đã hủy đơn hàng thành công.");
        }
    }
}
