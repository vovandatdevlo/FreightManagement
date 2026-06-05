using FreightManagement.Models;

namespace FreightManagement.Repositories.RepoInterfaces
{
    public interface IHangTrongKhosRepository
    {
        Task WarehouseAdd(HangTrongKho h);
        Task<List<HangTrongKho>> WarehouseGetToStock();
        Task<List<HangTrongKho>> WarehouseGetToExported();
        Task<List<HangTrongKho>> GetStockByMaQLK(int maQLK);

        Task<List<HangTrongKho>> GetExportedByMaQLK(int maQLK);
        Task<HangTrongKho?> GetHangTrongKhoByMaDon(int maDon);
        Task UpdateHangTrongKho(HangTrongKho h);
    }
}
