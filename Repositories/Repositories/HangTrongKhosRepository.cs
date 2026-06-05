using FreightManagement.Models;
using FreightManagement.Data;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Repositories.RepoInterfaces;
using System.Runtime.CompilerServices;

namespace FreightManagement.Repositories.Repository
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
        public async Task<List<HangTrongKho>> GetStockByMaQLK(int maQLK)
        {
            return await _db.HangTrongKhos
                .Include(h => h.KhoHang)
                .Include(h => h.DonHang).ThenInclude(d => d!.KhachHang)
                .Include(h => h.DonHang).ThenInclude(d => d!.TaiXe)
                .Where(h => h.KhoHang!.MaQLK == maQLK) 
                .Where(h => h.ThoiGianXuatKho == null)
                .Where(h => h.DonHang.TrangThai == "Đã vào kho")
                .OrderBy(h => h.ThoiGianVaoKho)
                .ToListAsync();
        }

        public async Task<List<HangTrongKho>> GetExportedByMaQLK(int maQLK)
        {
            return await _db.HangTrongKhos
                .Include(h => h.KhoHang)
                .Include(h => h.DonHang).ThenInclude(d => d!.KhachHang)
                .Include(h => h.DonHang).ThenInclude(d => d!.TaiXe)
                .Where(h => h.KhoHang!.MaQLK == maQLK)   
                .Where(h => h.DonHang.TrangThai == "Đang vận chuyển")
                .OrderByDescending(h => h.ThoiGianXuatKho)
                .ToListAsync();
        }
        public async Task<HangTrongKho?> GetHangTrongKhoByMaDon(int maDon)
        {
            return await _db.HangTrongKhos
                .FirstOrDefaultAsync(d => d.MaDon == maDon)!;
        }

        public async Task UpdateHangTrongKho(HangTrongKho h)
        {
            _db.HangTrongKhos.Update(h);
            await _db.SaveChangesAsync();
        }
    }
}
