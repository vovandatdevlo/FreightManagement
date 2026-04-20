using FreightManagement.DTOs;
using FreightManagement.MainRepositories;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.Models;

namespace FreightManagement.Service
{
    public class OrdersService
    {
        IDonHangsRepository _OrdersRepo;
        IUsersRepository _UsersRepo;
        ILichSuTrangThaisRepository _hisRepo;
        public OrdersService(IDonHangsRepository OrdersRepo, IUsersRepository UsersRepo, ILichSuTrangThaisRepository hisRepo)
        {
            _OrdersRepo = OrdersRepo;
            _UsersRepo = UsersRepo;
            _hisRepo = hisRepo;
        }
        public async Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid)
        {
            return await _OrdersRepo.OrdersGetDonHangsByMaKHAndDescending(uid);
        }
        public async Task<User> GetUserById(int UserId)
        {
            return await _UsersRepo.GetUserById(UserId);
        }
        public async Task<string> CreateOrderService(int userId, CreateOrderDTO obj)
        {
            int donGia = 0;
            var user = await _UsersRepo.GetUserById(userId);
            if (obj.LoaiHang == "HangHoa")
                donGia = 25000;
            else if (obj.LoaiHang == "ThuTu")
                donGia = 20000;
            else
                donGia = 30000;
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
            if (dh == null)
                return (false, null!);
            else
                return (true, dh);
        }
        public async Task<(bool IsValidOrder, string message)> CancelService(int id, int userId)
        {
            var order = await _OrdersRepo.OrdersGetDonHangsByMaDonAndMaKH(id, userId);
            if (order == null || order.TrangThai != "Đang xử lý")
                return (false, "Không thể hủy đơn hàng này.");
            order.TrangThai = "Đã hủy";
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
