using FreightManagement.Data;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.Models;

namespace FreightManagement.MainRepositories.Repository
{
    public class LichSuTrangThaisRepository : ILichSuTrangThaisRepository
    {
        private readonly AppDbContext _db;
        public LichSuTrangThaisRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task DriverAddLichSuTrangThai(LichSuTrangThai ls)
        {
            _db.LichSuTrangThais.Add(ls);
            await _db.SaveChangesAsync();
        }
    }
}
