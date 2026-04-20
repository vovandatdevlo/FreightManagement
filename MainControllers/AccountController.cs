using FreightManagement.Data;
using FreightManagement.Filters;
using FreightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using FreightManagement.Service;
using FreightManagement.DTOs;

namespace FreightManagement.MainControllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _acc;
        public AccountController(AccountService acc)
        {
            _acc = acc;
        }
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (_acc.IsInValidAccount(email, password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var user = await _acc.GetUserToLogin(email, password);

            if (_acc.IsNullObject(user))
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.";
                return View();
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("RoleName", user.Role.RoleName);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);

            // Điều hướng theo role
            return user.Role.RoleName switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "KhachHang" => RedirectToAction("Index", "Orders"),
                "QuanLyKho" => RedirectToAction("Index", "Warehouse"),
                "TaiXe" => RedirectToAction("Index", "Driver"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // --------------------------------------------------
        // ĐĂNG KÝ (chỉ cho KhachHang tự đăng ký)
        // --------------------------------------------------
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterDTO obj)
        {
            
            if (obj.password != obj.confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (await _acc.IsExistObject(obj.email))
            {
                ViewBag.Error = "Email này đã được đăng ký.";
                return View();
            }

            var user = new User
            {
                HoTen = obj.hoTen,
                Email = obj.email,
                PasswordHash = obj.password,
                SoDienThoai = obj.soDienThoai,
                DiaChi = obj.diaChi,
                RoleId = 2  // KhachHang
            };

            await _acc.AddUser(user);

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // --------------------------------------------------
        // ĐĂNG XUẤT
        // --------------------------------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // --------------------------------------------------
        // HELPER: Hash mật khẩu bằng SHA256
        // Ghi chú: Thực tế nên dùng BCrypt.Net-Next (an toàn hơn)
        //   Install-Package BCrypt.Net-Next
        //   BCrypt.Net.BCrypt.HashPassword(password)
        //   BCrypt.Net.BCrypt.Verify(password, hash)
        // --------------------------------------------------
        //private static string HashPassword(string password)
        //{
        //    // service
        //    using var sha = SHA256.Create();
        //    var bytes = Encoding.UTF8.GetBytes(password);
        //    var hash = sha.ComputeHash(bytes);
        //    return Convert.ToHexString(hash);
        //}

        // ── XEM THÔNG TIN CÁ NHÂN ──────────────────────────────────────────
        [RequireLogin]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;

            var user = await _acc.GetUserByRoleAndId(userId);

            if (_acc.IsNullObject(user)) return RedirectToAction("Login");
            return View(user);
        }

        // ── CẬP NHẬT THÔNG TIN CÁ NHÂN ─────────────────────────────────────
        [HttpPost]
        [RequireLogin]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO obj)
        {
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;

            var user = await _acc.GetUserById(userId);

            if (_acc.IsNullObject(user)) return RedirectToAction("Login");

            // Validate họ tên không được rỗng

            if (_acc.IsEmptyHoTen(obj.hoTen))
            {
                var u = await _acc.GetFirstUserById(userId);
                ViewBag.Error = "Họ tên không được để trống.";
                return View("Profile", u);
            }

            // Cập nhật thông tin

            // CCCD chỉ cập nhật cho TaiXe
            var roleName = HttpContext.Session.GetString("RoleName");

            await _acc.UpdateInFor(roleName, user, obj);

            // Cập nhật tên trong Session
            HttpContext.Session.SetString("HoTen", user.HoTen);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        // ── XEM ĐỔI MẬT KHẨU ──────────────────────────────────────────────
        [RequireLogin]
        public IActionResult ChangePassword() => View();

        // ── ĐỔI MẬT KHẨU ───────────────────────────────────────────────────
        [HttpPost]
        [RequireLogin]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO obj)
        {
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _acc.ChangePasswordMessage(userId, obj);
            if (!result.success)
            {
                ViewBag.Error = result.message;
                return View();
            }
            else
            {
                TempData["Success"] = result.message;
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
        }
    }
}
