using FreightManagement.Models;
using FreightManagement.DTOs;

namespace FreightManagement.Services.IServices
{
    public interface IOrdersService
    {
        Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid);
        Task<Users> GetUserById(int UserId);
        Task<string> CreateOrderService(int userId, CreateOrderDTO obj);
        Task<(bool IsValidOrder, DonHang order)> DetailOrderService(int id, int userId);
        Task<(bool IsValidOrder, string message)> CancelService(int id, int userId);
    }
}
