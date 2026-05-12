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
        Task<Users> GetUserByAccount(string email, string passwordHash);
        Task<bool> CheckExistUserByEmail(string email);
        Task AddUser(Users u);
        Task<Users> GetUserByRoleAndUserID(int UserId);
        Task<Users> GetUserById(int UserId);
        Task<Users> GetFirstUserById(int UserId);
        Task UpdateUser(Users user);
        Task<int> GetUsersCountByRoleId(int roleId);
        Task<List<Users>> GetUsersList();
        Task<List<Users>> GetTaixeByKho(string diachi);
    }
}
