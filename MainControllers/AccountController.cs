using FreightManagement.DTOs;
using FreightManagement.Models;
using FreightManagement.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreightManagement.MainControllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _acc;
        public AccountController(AccountService acc) 
        {
            _acc = acc; 
        }

        // ── ĐĂNG NHẬP ──────────────────────────────────────────────
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

            // Tạo Claims cho Cookie Authentication
            var claims = new List<Claim>
            {
                new Claim("UserId",   user.UserId.ToString()),
                new Claim("HoTen",    user.HoTen),
                new Claim("Email",    user.Email),
                new Claim(ClaimTypes.Role,  user.Role.RoleName),
                new Claim("RoleId",   user.RoleId.ToString()),
                new Claim(ClaimTypes.Name,  user.Email),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return user.Role.RoleName switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "KhachHang" => RedirectToAction("Index", "Orders"),
                "QuanLyKho" => RedirectToAction("Index", "Warehouse"),
                "TaiXe" => RedirectToAction("Index", "Driver"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // ── ĐĂNG KÝ ────────────────────────────────────────────────
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(AccountRegisterDTO obj)
        {
            //if (obj.password != obj.confirmPassword)
            //{
            //    ViewBag.Error = "Mật khẩu xác nhận không khớp.";
            //    return View();
            //}

            //if (await _acc.IsExistObject(obj.email))
            //{
            //    ViewBag.Error = "Email này đã được đăng ký.";
            //    return View();
            //}

            //var user = new User
            //{
            //    HoTen = obj.hoTen,
            //    Email = obj.email,
            //    PasswordHash = obj.password,
            //    SoDienThoai = obj.soDienThoai,
            //    DiaChi = obj.diaChi,
            //    RoleId = 2 // KhachHang
            //};

            //await _acc.AddUser(user);
            //TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            var result = await _acc.RegisterService(obj);
            if (!result.isValidInfor)
            {
                ViewBag.Error = result.message;
                return View();
            }
            TempData["Success"] = result.message;
            return RedirectToAction("Login");
        }

        // ── ĐĂNG XUẤT ──────────────────────────────────────────────
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ── ACCESS DENIED ──────────────────────────────────────────
        public IActionResult AccessDenied() => View();

        // ── THÔNG TIN CÁ NHÂN ──────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _acc.GetUserByRoleAndId(userId);
            if (_acc.IsNullObject(user)) return RedirectToAction("Login");
            return View(user);
        }

        // ── CẬP NHẬT THÔNG TIN ─────────────────────────────────────
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDTO obj)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _acc.GetUserById(userId);
            if (_acc.IsNullObject(user)) return RedirectToAction("Login");

            if (_acc.IsEmptyHoTen(obj.hoTen!))
            {
                var u = await _acc.GetFirstUserById(userId);
                ViewBag.Error = "Họ tên không được để trống.";
                return View("Profile", u);
            }

            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            await _acc.UpdateInFor(roleName!, user, obj);
            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }

        // ── ĐỔI MẬT KHẨU ───────────────────────────────────────────
        [Authorize]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO obj)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _acc.ChangePasswordMessage(userId, obj);
            if (!result.success)
            {
                ViewBag.Error = result.message;
                return View();
            }
            TempData["Success"] = result.message;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}