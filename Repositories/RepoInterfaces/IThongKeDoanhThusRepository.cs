using FreightManagement.Models;

namespace FreightManagement.Repositories.RepoInterfaces
{
    public interface IThongKeDoanhThusRepository
    {
        Task<int> AdminGetSum();
        Task<List<ThongKeDoanhThu>> AdminGetToRevenue();
        Task AddThongKeDoanhThu(ThongKeDoanhThu tkdt);
    }
}
