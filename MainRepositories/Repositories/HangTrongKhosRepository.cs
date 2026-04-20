using FreightManagement.Models;
using FreightManagement.Data;
using Microsoft.EntityFrameworkCore;
using FreightManagement.MainRepositories.RepoInterfaces;

namespace FreightManagement.MainRepositories.Repository
{
    public class HangTrongKhosRepository : IHangTrongKhosRepository
    {
        private readonly AppDbContext _db;
        public HangTrongKhosRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task WarehouseAdd(HangTrongKho h)
        {
            _db.HangTrongKhos.Add(h);
            await _db.SaveChangesAsync();
        }
        public async Task<List<HangTrongKho>> WarehouseGetToStock()
        {
            return await _db.HangTrongKhos
                .Include(h => h.KhoHang)
                .Include(h => h.DonHang).ThenInclude(d => d.KhachHang)
                .Include(h => h.DonHang).ThenInclude(d => d.TaiXe)
                .Where(h => h.ThoiGianXuatKho == null)
                .ToListAsync();
        }
        public async Task<List<HangTrongKho>> WarehouseGetToExported()
        {
            return await _db.HangTrongKhos
                .Include(h => h.KhoHang)
                .Include(h => h.DonHang).ThenInclude(d => d.KhachHang)
                .Where(h => h.ThoiGianXuatKho != null)
                .OrderByDescending(h => h.ThoiGianXuatKho)
                .ToListAsync();
        }
    }
}
