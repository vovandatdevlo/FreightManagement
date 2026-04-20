using FreightManagement.Data;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Models;
using FreightManagement.MainRepositories.RepoInterfaces;

namespace FreightManagement.MainRepositories.Repository
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
    }
}
