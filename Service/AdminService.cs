using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.MainRepositories.Repository;
using FreightManagement.Models;
namespace FreightManagement.Service
{
    public class AdminService
    {
        // Kiểm tra Interface
        private readonly IDonHangsRepository _OrderRepo;
        private readonly IUsersRepository _UsersRepo;
        private readonly IKhoHangsRepository _WareRepo;
        private readonly IThongKeDoanhThusRepository _TKRepo;
        public AdminService(IDonHangsRepository OrderRepo, IUsersRepository UsersRepo, IKhoHangsRepository WareRepo, IThongKeDoanhThusRepository TKRepo)
        {
            _OrderRepo = OrderRepo;
            _UsersRepo = UsersRepo;
            _WareRepo = WareRepo;
            _TKRepo = TKRepo;
        }
        public async Task<int> GetDonHangsCount()
        {
            return await _OrderRepo.GetDonHangsCount();
        }
        public async Task<int> GetDonHangsCountByTrangThai(string trangthai)
        {
            return await _OrderRepo.GetDonHangsCountByTrangThai(trangthai);
        }
        public async Task<int> GetUsersCountByRoleId(int roleId)
        {
            return await _UsersRepo.GetUsersCountByRoleId(roleId);
        }
        public async Task<int> AdminGetTongKho()
        {
            return await _WareRepo.AdminGetTongKho();
        }
        public async Task<int> AdminGetSum()
        {
            return await _TKRepo.AdminGetSum();
        }
        public async Task<List<DonHang>> GetRecentDonHangs()
        {
            return await _OrderRepo.GetRecentDonHangs();
        }
        public async Task<List<User>> GetUsersList()
        {
            return await _UsersRepo.GetUsersList();
        }
        public async Task<(bool IsValidUser, string message)> ToggleAccountService(int id)
        {
            var user = await _UsersRepo.GetUserById(id);
            if (user != null && user.RoleId != 1)
            {
                user.TrangThai = user.TrangThai == "HoatDong" ? "BiKhoa" : "HoatDong";
                await _UsersRepo.UpdateUser(user);
                if (user.TrangThai == "HoatDong")
                    return (true, $"Đã mở khóa tài khoản {user.HoTen}.");
                else
                    return (true, $"Đã khóa tài khoản {user.HoTen}.");
            }
            return (false, "");
        }
        public async Task<List<DonHang>> GetFullOrders()
        {
            return await _OrderRepo.GetFullOrders();
        }
        public async Task<List<KhoHang>> AdminGetKhoHangToWarehouses()
        {
            return await _WareRepo.AdminGetKhoHangToWarehouses();
        }
        public async Task<List<ThongKeDoanhThu>> AdminGetToRevenue()
        {
            return await _TKRepo.AdminGetToRevenue();
        }
    }
}
