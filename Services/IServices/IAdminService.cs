using FreightManagement.DTOs;
using FreightManagement.Models;

namespace FreightManagement.Services.IServices
{
    public interface IAdminService
    {
        Task<int> GetDonHangsCount();
        Task<int> GetDonHangsCountByTrangThai(string trangthai);
        Task<int> GetUsersCountByRoleId(int roleId);
        Task<int> AdminGetTongKho();
        Task<int> AdminGetSum();
        Task<List<DonHang>> GetRecentDonHangs();
        Task<List<Users>> GetUsersList();
        Task<(bool IsValidUser, string message)> ToggleAccountService(int id);
        Task<(bool success, string message)> AddStaffService(AddStaffDTO dto);
        Task<(List<DonHang> items, int totalCount)> GetFullOrdersFiltered(
            string? trangThai, string? tuNgay, string? denNgay,
            string? tuKhoa, int page, int pageSize = 20
            );
        Task<List<KhoHang>> AdminGetKhoHangToWarehouses();
        Task<List<Users>> GetAllQuanLyKho();
        Task<(bool success, string message)> AssignKhoToQuanLyKho(int maKho, int maQLK);
        Task<List<ThongKeDoanhThu>> AdminGetToRevenue();
        Task<(bool success, string message)> UpdateDriverCCCD(int driverId, string cccd);
    }
}
