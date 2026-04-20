using FreightManagement.Models;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface ILichSuTrangThaisRepository
    {
        public Task DriverAddLichSuTrangThai(LichSuTrangThai ls);
    }
}
