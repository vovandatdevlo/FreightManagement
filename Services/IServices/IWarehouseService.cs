using FreightManagement.Models;

namespace FreightManagement.Services.IServices
{
    public interface IWarehouseService
    {
        Task<int> GetDonHangsCountByTrangThai(string trangthai, int MaQLK);
        Task<int> WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat(string trangthai, int MaQLK);
        Task<(List<DonHang> orders, List<KhoHang> Warehouses)> IncomingService(string trangthai, int uid);
        Task<(bool IsValidItem, string message)> NhanVaoKhoService(int uid, int madon, int makho);
        Task<List<KhoHang>> QuanLyKhoGetMyWarehouses(int uid);
        Task<(Dictionary<int, List<Users>>, List<DonHang> ordersList)> AssignService(int MaQLK);
        Task<(bool valid, string message)> GanTaiXeService(int uid, int maDon, int maTX);
        Task<List<DonHang>> GetDonHangsGiaoThatBai();
        Task<(bool valid, string message)> XuLyGiaoLai(int maDon, int uid);
        Task<List<HangTrongKho>> StockService(int uid);
        Task<List<HangTrongKho>> ExportedService(int uid);
    }
}
