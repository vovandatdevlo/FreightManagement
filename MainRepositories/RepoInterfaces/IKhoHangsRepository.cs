using FreightManagement.Models;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IKhoHangsRepository
    {
        Task<int> AdminGetTongKho();
        Task<List<KhoHang>> AdminGetKhoHangToWarehouses();
        Task<List<KhoHang>> WarehouseGetKhoHangToIncoming(int uid);
        Task<KhoHang> WarehouseGetKhoHangToNhanVaoKho(int maKho);
        // Gán kho cho Quản lý kho
        Task<KhoHang> GetKhoHangById(int maKho);
        Task UpdateKhoHang(KhoHang khoHang);
        Task<List<KhoHang>> GetKhoHangByMaQLK(int maQLK);

        // Admin quản lý
        Task<List<User>> GetAllQuanLyKho();
    }
}
