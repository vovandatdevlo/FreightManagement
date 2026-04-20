using FreightManagement.Models;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IKhoHangsRepository
    {
        Task<int> AdminGetTongKho();
        Task<List<KhoHang>> AdminGetKhoHangToWarehouses();
        Task<List<KhoHang>> WarehouseGetKhoHangToIncoming(int uid);
        Task<KhoHang> WarehouseGetKhoHangToNhanVaoKho(int maKho);
    }
}
