using FreightManagement.Models;
using FreightManagement.DTOs;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IDonHangsRepository
    {
        Task UpdateOrder(DonHang order);
        Task<int> GetDonHangsCount();
        Task<int> GetDonHangsCountByTrangThai(string trangthai, int MaQLK);
        Task<int> GetFullOrdersCountByTrangThai(string trangthai);
        Task<List<DonHang>> GetRecentDonHangs();
        //Task<List<DonHang>> 
        Task<List<DonHang>> GetFullOrdersDangXuLy();

        // YC5: filter + phân trang
        Task<(List<DonHang> items, int totalCount)> GetFullOrdersFiltered(
            string? trangThai, string? tuNgay, string? denNgay,
            string? tuKhoa, int page, int pageSize);

        // YC4: lấy theo trạng thái bất kỳ (dùng cho "Giao thất bại")
        Task<List<DonHang>> GetDonHangsByTrangThai(string trangthai);

        Task<int> GetDonHangsCountByTrangThaiAndId(int uid, string trangthai);
        Task<List<DonHang>> TaixeGetMyDonHangs(int uid, string trangthai);
        Task<DonHang> TaixeGetDonHangsByMadonAndMaTX(int madon, int MaTX);
        Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThai(int MaTX, string trangthai);
        Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai);
        Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid);
        Task OrdersAddDonHangs(DonHang order);
        Task<DonHang> OrdersGetDonHangsIncludeByMaDonAndMaKH(int id, int userId);
        Task<DonHang> OrdersGetDonHangsByMaDonAndMaKH(int id, int userId);
        Task<int> WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat(string trangthai, int MaQLK);

        Task<List<DonHang>> WarehouseGetDonHangsToIncoming(string trangthai);

        Task<DonHang> WarehouseGetDonHangsToNhanVaoKho(int madon);
        Task<List<DonHang>> WarehouseGetDonHangsToAssignOrders(string trangthai);
        Task<List<Taixetheokho>> WarehouseGetDonHangsToAssignTaixetheokho(string trangthai);
        Task<bool> CheckExistSdtNguoiNhan(string sdt);
    }
}