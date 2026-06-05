using FreightManagement.Models;

namespace FreightManagement.Repositories.RepoInterfaces
{
    public interface ILichSuTrangThaisRepository
    {
        public Task DriverAddLichSuTrangThai(LichSuTrangThai ls);
    }
}
