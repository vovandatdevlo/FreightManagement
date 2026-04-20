using FreightManagement.Models;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IThongKeDoanhThusRepository
    {
        Task<int> AdminGetSum();
        Task<List<ThongKeDoanhThu>> AdminGetToRevenue();
    }
}
