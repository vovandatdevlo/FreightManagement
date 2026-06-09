using FreightManagement.DTOs;
using FreightManagement.Repositories;
using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Models;
using FreightManagement.Services.IServices;

namespace FreightManagement.Services.Service
{
    public class OrdersService : IOrdersService
    {
        private readonly IDonHangsRepository _OrdersRepo;
        private readonly IUsersRepository _UsersRepo;
        private readonly ILichSuTrangThaisRepository _hisRepo;
        private readonly IKhoHangsRepository _WareRepo;

        public OrdersService(IDonHangsRepository OrdersRepo, IUsersRepository UsersRepo, ILichSuTrangThaisRepository hisRepo, IKhoHangsRepository WareRepo)
        {
            _OrdersRepo = OrdersRepo;
            _UsersRepo = UsersRepo;
            _hisRepo = hisRepo;
            _WareRepo = WareRepo;

        }

        public async Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid)
            => await _OrdersRepo.OrdersGetDonHangsByMaKHAndDescending(uid);

        // Trường

        public async Task<Users> GetUserById(int UserId)
            => await _UsersRepo.GetUserById(UserId);

        // Trường

        public async Task<string> CreateOrderService(int userId, CreateOrderDTO obj)
        {
            var sdt = obj.SdtNguoiNhan?.Trim() ?? "";
            if (sdt.Length != 10 || !sdt.All(char.IsDigit))
                return "Số điện thoại người nhận không hợp lệ (phải đúng 10 số).";

            int donGia = obj.LoaiHang switch
            {
                "HangHoa" => 25000,
                "ThuTu" => 20000,
                "Fresh" => 30000,
                _ => 50000,
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
            return $"Đặt đơn hàng #DH{order.MaDon:D5} thành công!";
        }

        public async Task<(bool IsValidOrder, DonHang order)> DetailOrderService(int id, int userId)
        {
            var dh = await _OrdersRepo.OrdersGetDonHangsIncludeByMaDonAndMaKH(id, userId);
            if (dh == null) return (false, null!);
            return (true, dh);
        }

        // Trường

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
