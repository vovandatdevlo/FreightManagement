using FreightManagement.Data;
using FreightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FreightManagement.MainRepositories.RepoInterfaces
{
    public interface IUsersRepository
    {
        Task<User> GetUserByAccount(string email, string passwordHash);
        Task<bool> CheckExistUserByEmail(string email);
        Task AddUser(User u);
        Task<User> GetUserByRoleAndUserID(int UserId);
        Task<User> GetUserById(int UserId);
        Task<User> GetFirstUserById(int UserId);
        Task UpdateUser(User user);
        Task<int> GetUsersCountByRoleId(int roleId);
        Task<List<User>> GetUsersList();
        Task<List<User>> GetTaixeByKho(string diachi);
    }
}
