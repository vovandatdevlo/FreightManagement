using FreightManagement.Models;
using FreightManagement.DTOs;

namespace FreightManagement.Services.IServices
{
    public interface IAccountService
    {
        bool IsInValidAccount(string email, string password);
        bool IsNullObject(Users user);
        Task<Users> GetUserToLogin(string email, string password);
        Task<Users> GetUserByRoleAndId(int userId);
        Task<Users> GetUserById(int userId);
        bool IsEmptyHoTen(string hoten);
        Task<Users> GetFirstUserById(int userId);
        Task<(bool isValidInfor, string message)> RegisterService(AccountRegisterDTO obj);
        Task<(bool success, string message)> UpdateInFor(string roleName, Users user, UpdateProfileDTO obj);
        Task<(bool success, string message)> ChangePasswordMessage(int userId, ChangePasswordDTO obj);
    }
}
