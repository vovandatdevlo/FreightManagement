using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Repositories.Repository;
using FreightManagement.Models;
using System.Security.Cryptography;
using System.Text;
using FreightManagement.DTOs;
using FreightManagement.Services.IServices;

namespace FreightManagement.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository _UsersRepo;
        public AccountService(IUsersRepository UsersRepo)
        {
            _UsersRepo = UsersRepo;
        }
        public bool IsInValidAccount(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return true;
            return false;
        }
        public bool IsNullObject(Users user)
        {
            if (user == null)
                return true;
            return false;
        }

        // Trường

        public async Task<bool> IsExistObject(string email)
        {
            if (await _UsersRepo.CheckExistUserByEmail(email))
                return true;
            return false;
        }
        public async Task<Users> GetUserToLogin(string email, string password)
        {
            var hash = HashPassword(password);
            return await _UsersRepo.GetUserByAccount(email, hash);
        }
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        // Trường

        public async Task AddUser(Users user)
        {
            user.PasswordHash = HashPassword(user.PasswordHash);
            await _UsersRepo.AddUser(user);
        }
        public async Task<Users> GetUserByRoleAndId(int userId)
        {
            return await _UsersRepo.GetUserByRoleAndUserID(userId);
        }
        public async Task<Users> GetUserById(int userId)
        {
            return await _UsersRepo.GetUserById(userId);
        }
        public bool IsEmptyHoTen(string hoten)
        {
            if (string.IsNullOrWhiteSpace(hoten))
                return true;
            return false;
        }
        public async Task<Users> GetFirstUserById(int userId)
        {
            return await _UsersRepo.GetFirstUserById(userId);
        }

        // Trường

        public async Task<(bool isValidInfor, string message)> RegisterService(AccountRegisterDTO obj)
        {
            if (obj.password != obj.confirmPassword)
            {
                return (false, "Mật khẩu xác nhận không khớp.");
            }
            if (await IsExistObject(obj.email))
            {
                return (false, "Email này đã được đăng ký.");
            }
            var user = new Users
            {
                HoTen = obj.hoTen,
                Email = obj.email,
                PasswordHash = obj.password,
                SoDienThoai = obj.soDienThoai,
                DiaChi = obj.diaChi,
                RoleId = 2,
            };
            await AddUser(user);
            return (true, "Đăng ký thành công! Vui lòng đăng nhập.");
        }
        public async Task<(bool success, string message)> UpdateInFor(string roleName, Users user, UpdateProfileDTO obj)
        {
            user.HoTen = obj.hoTen!.Trim();

            // Kiểm tra trùng SĐT (chỉ với tài xế, không tính chính mình)
            if (roleName == "TaiXe" && !string.IsNullOrWhiteSpace(obj.soDienThoai))
            {
                if (await _UsersRepo.CheckExistDriverBySoDienThoai(obj.soDienThoai.Trim(), user.UserId))
                    return (false, "Số điện thoại này đã được sử dụng bởi tài xế khác.");
            }
            if (!string.IsNullOrWhiteSpace(obj.soDienThoai))
            {
                var sdt = obj.soDienThoai.Trim();
                if (sdt.Length != 10 || !sdt.All(char.IsDigit))
                    return (false, "Số điện thoại không hợp lệ (phải đúng 10 số).");
            }

            user.SoDienThoai = obj.soDienThoai?.Trim();
            user.DiaChi = obj.diaChi?.Trim();

            // CCCD của tài xế KHÔNG được tự chỉnh sửa — chỉ admin mới cập nhật được
            // (không cập nhật CCCD ở đây dù roleName == "TaiXe")

            await _UsersRepo.UpdateUser(user);
            return (true, "Cập nhật thông tin thành công!");
        }

        // Trường

        public async Task<(bool success, string message)> ChangePasswordMessage(int userId, ChangePasswordDTO obj)
        {
            var user = await _UsersRepo.GetUserById(userId);
            if (string.IsNullOrWhiteSpace(obj.newPassword) || obj.newPassword.Length < 6)
            {
                return (false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
            }
            if (obj.newPassword != obj.confirmPassword)
                return (false, "Mật khẩu xác nhận không khớp.");
            if (user == null || user.PasswordHash != HashPassword(obj.currentPassword!))
                return (false, "Mật khẩu hiện tại không đúng.");
            if (HashPassword(obj.newPassword) == user.PasswordHash)
                return (false, "Mật khẩu mới không được trùng mật khẩu cũ.");
            user.PasswordHash = HashPassword(obj.newPassword);
            await _UsersRepo.UpdateUser(user);
            return (true, "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");
        }
    }
}
