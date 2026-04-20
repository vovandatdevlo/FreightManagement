using FreightManagement.Models;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IHangTrongKhosRepository
    {
        Task WarehouseAdd(HangTrongKho h);
        Task<List<HangTrongKho>> WarehouseGetToStock();
        Task<List<HangTrongKho>> WarehouseGetToExported();
    }
}
