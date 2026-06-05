using FreightManagement.Data;
using FreightManagement.Models;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Repositories.RepoInterfaces;

namespace FreightManagement.Repositories.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _db;
        public UsersRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Users?> GetUserByAccount(string email, string passwordHash)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash && u.TrangThai == "HoatDong");
        }

        // Trường

        public async Task<bool> CheckExistUserByEmail(string email)
        {
            return await _db.Users
                .AnyAsync(u => u.Email == email);
        }
        public async Task AddUser(Users u)
        {
            _db.Users.Add(u);
            await _db.SaveChangesAsync();
        }
        public async Task<Users?> GetUserByRoleAndUserID(int UserId)
        {
            return await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == UserId);
        }
        public async Task<Users?> GetUserById(int UserId)
        {
            return await _db.Users
                .FindAsync(UserId);
        }
        public async Task<Users> GetFirstUserById(int userId)
        {
            return await _db.Users.Include(d => d.Role).FirstAsync(d => d.UserId == userId);
        }
        public async Task<int> GetUsersCountByRoleId(int roleId)
        {
            return await _db.Users
                .CountAsync(u => u.RoleId == roleId);
        }
        public async Task<List<Users>> GetUsersList()
        {
            return await _db.Users
                .Include(u => u.Role)
                .OrderBy(u => u.RoleId)
                .ThenBy(u => u.HoTen)
                .ToListAsync();
        }
        public async Task<List<Users>> GetTaixeByKho(string diachi)
        {
            return await _db.Users
                .Where(u => u.RoleId == 4 && u.TrangThai == "HoatDong" && u.DiaChi == diachi)
                .ToListAsync();
        }

        public async Task<bool> CheckExistDriverBySoDienThoai(string soDienThoai, int? excludeUserId = null)
        {
            return await _db.Users
                .AnyAsync(u => u.RoleId == 4
                            && u.SoDienThoai == soDienThoai
                            && (excludeUserId == null || u.UserId != excludeUserId.Value));
        }

        public async Task<bool> CheckExistDriverByCCCD(string cccd, int? excludeUserId = null)
        {
            return await _db.Users
                .AnyAsync(u => u.RoleId == 4
                            && u.CCCD == cccd
                            && (excludeUserId == null || u.UserId != excludeUserId.Value));
        }
        public async Task UpdateUser(Users user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
