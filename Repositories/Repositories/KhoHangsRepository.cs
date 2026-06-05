using FreightManagement.Data;
using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FreightManagement.Repositories.Repository
{
    public class KhoHangsRepository : IKhoHangsRepository
    {
        private readonly AppDbContext _db;

        public KhoHangsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<KhoHang>> AdminGetKhoHangToWarehouses()
        {
            return await _db.KhoHangs
                .Include(k => k.QuanLyKho)
                .OrderBy(k => k.TenKho)
                .ToListAsync();
        }

        public async Task<List<KhoHang>> WarehouseGetKhoHangToIncoming(int uid)
        {
            return await _db.KhoHangs
                .Where(k => k.MaQLK == uid)
                .ToListAsync();
        }

        public async Task<KhoHang> WarehouseGetKhoHangToNhanVaoKho(int makho)
        {
            return await _db.KhoHangs.FindAsync(makho);
        }

        public async Task<int> AdminGetTongKho()
        {
            return await _db.KhoHangs.CountAsync();
        }

        public async Task<KhoHang> GetKhoHangById(int maKho)
        {
            return await _db.KhoHangs.FindAsync(maKho);
        }

        public async Task UpdateKhoHang(KhoHang khoHang)
        {
            _db.KhoHangs.Update(khoHang);
            await _db.SaveChangesAsync();
        }

        public async Task<List<KhoHang>> GetKhoHangByMaQLK(int maQLK)
        {
            return await _db.KhoHangs
                .Where(k => k.MaQLK == maQLK)
                .Include(k => k.QuanLyKho)
                .ToListAsync();
        }


        public async Task<List<Users>> GetAllQuanLyKho()
        {
            return await _db.Users
                .Where(u => u.RoleId == 3)
                .Where(u => u.TrangThai == "HoatDong")
                .OrderBy(u => u.HoTen)
                .ToListAsync();
        }
    }
}
