using FreightManagement.Data;
using FreightManagement.Models;
using Microsoft.EntityFrameworkCore;
using FreightManagement.MainRepositories.RepoInterfaces;

namespace FreightManagement.MainRepositories.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _db;
        public UsersRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<User> GetUserByAccount(string email, string passwordHash)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash && u.TrangThai == "HoatDong");   
        }
        public async Task<bool> CheckExistUserByEmail(string email)
        {
            return await _db.Users
                .AnyAsync(u => u.Email == email);
        }
        public async Task AddUser(User u)
        {
            _db.Users.Add(u);
            await _db.SaveChangesAsync();
        }
        public async Task<User> GetUserByRoleAndUserID(int UserId)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == UserId);
        }
        public async Task<User> GetUserById(int UserId)
        {
            return await _db.Users
                .FindAsync(UserId);
        }
        public async Task<User> GetFirstUserById(int userId)
        {
            return await _db.Users.Include(d => d.Role).FirstAsync(d => d.UserId == userId);
        }
        public async Task<int> GetUsersCountByRoleId(int roleId)
        {
            return await _db.Users
                .CountAsync(u => u.RoleId == roleId);
        }
        public async Task<List<User>> GetUsersList()
        {
            return await _db.Users
                .Include(u => u.Role)
                .OrderBy(u => u.RoleId)
                .ThenBy(u => u.HoTen)
                .ToListAsync();
        }
        public async Task<List<User>> GetTaixeByKho(string diachi)
        {
            return await _db.Users
                .Where(u => u.RoleId == 4 && u.TrangThai == "HoatDong" && u.DiaChi == diachi)
                .ToListAsync();
        }
        public async Task UpdateUser(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
