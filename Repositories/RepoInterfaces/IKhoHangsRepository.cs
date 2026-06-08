using FreightManagement.Models;

namespace FreightManagement.Repositories.RepoInterfaces
{
    public interface IKhoHangsRepository
    {
        Task<int> AdminGetTongKho();
        Task<List<KhoHang>> AdminGetKhoHangToWarehouses();
        Task<List<KhoHang>> QuanLyKhoGetMyWarehouses(int uid);
        Task<List<KhoHang>> WarehouseGetKhoHangToIncoming(int uid);
        Task<KhoHang> GetKhoHangByDiaChiKho(string diachi);
        //Task<KhoHang> WarehouseGetKhoHangToNhanVaoKho(int maKho);
        // Gán kho cho Quản lý kho
        Task<KhoHang> GetKhoHangById(int maKho);
        Task UpdateKhoHang(KhoHang khoHang);
        Task<List<KhoHang>> GetKhoHangByMaQLK(int maQLK);

        // Admin quản lý
        Task<List<Users>> GetAllQuanLyKho();
    }
}
