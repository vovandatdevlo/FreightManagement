using FreightManagement.Data;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FreightManagement.MainRepositories.Repository
{
    public class KhoHangsRepository : IKhoHangsRepository
    {
        private readonly AppDbContext _db;
        public KhoHangsRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<int> AdminGetTongKho()
        {
            return await _db.KhoHangs
                .CountAsync();
        }
        public async Task<List<KhoHang>> AdminGetKhoHangToWarehouses()
        {
            return await _db.KhoHangs
                .Include(k => k.QuanLyKho)
                .ToListAsync();
        }
        public async Task<List<KhoHang>> WarehouseGetKhoHangToIncoming(int uid)
        {
            return await _db.KhoHangs
                .Where(k => k.MaQLK == uid)
                .ToListAsync();
        }
        public async Task<KhoHang> WarehouseGetKhoHangToNhanVaoKho(int maKho)
        {
            return await _db.KhoHangs
                .FindAsync(maKho);
        }
    }
}
