using FreightManagement.Data;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Models;
using FreightManagement.Repositories.RepoInterfaces;

namespace FreightManagement.Repositories.Repository
{
    public class ThongKeDoanhThusRepository : IThongKeDoanhThusRepository
    {
        private readonly AppDbContext _db;
        public ThongKeDoanhThusRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<int> AdminGetSum()
        {
            return await _db.ThongKeDoanhThus
                .SumAsync(t => (int?)t.ChiPhiVanChuyen) ?? 0;
        }
        public async Task<List<ThongKeDoanhThu>> AdminGetToRevenue()
        {
            return await _db.ThongKeDoanhThus
                .Include(t => t.DonHang)
                .OrderByDescending(t => t.Ngay)
                .ToListAsync();
        }
        public async Task AddThongKeDoanhThu(ThongKeDoanhThu tkdt)
        {
            _db.ThongKeDoanhThus.Add(tkdt);
            await _db.SaveChangesAsync();
        }
    }
}
