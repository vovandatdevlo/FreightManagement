// =====================================================================
// Controllers/AdminController.cs — ĐẦY ĐỦ (thay thế file cũ)
// =====================================================================
using FreightManagement.Data;
using FreightManagement.Filters;
using FreightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Service;

namespace FreightManagement.MainControllers
{
    [RequireLogin("Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _ads;
        public AdminController(AdminService ads)
        {
            _ads = ads;
        }

        // ── DASHBOARD ──────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            // Thống kê đơn hàng
            ViewBag.TongDonHang = await _ads.GetDonHangsCount();
            ViewBag.DangXuLy = await _ads.GetDonHangsCountByTrangThai("Đang xử lý");
            ViewBag.DangVanChuyen = await _ads.GetDonHangsCountByTrangThai("Đang vận chuyển");
            ViewBag.DaGiao = await _ads.GetDonHangsCountByTrangThai("Đã giao");

            // Thống kê hệ thống
            ViewBag.TongKhachHang = await _ads.GetUsersCountByRoleId(2);
            ViewBag.TongTaiXe = await _ads.GetUsersCountByRoleId(4);
            ViewBag.TongKho = await _ads.AdminGetTongKho();
            ViewBag.DoanhThu = await _ads.AdminGetSum();

            // 10 đơn hàng gần nhất
            ViewBag.RecentOrders = await _ads.GetRecentDonHangs();

            return View();
        }

        // ── QUẢN LÝ USERS ──────────────────────────────────────────
        public async Task<IActionResult> Users()
        {
            var users = await _ads.GetUsersList();
            return View(users);
        }

        // Khóa / mở khóa tài khoản
        public async Task<IActionResult> ToggleUser(int id)
        {
            var result = await _ads.ToggleAccountService(id);
            if (result.IsValidUser)
            {
                TempData["Success"] = result.message;
            }
            return RedirectToAction("Users");
        }

        // ── XEM TẤT CẢ ĐƠN HÀNG ──────────────────────────────────
        public async Task<IActionResult> Orders()
        {
            var orders = await _ads.GetFullOrders();
            return View(orders);
        }

        // ── KHO HÀNG ──────────────────────────────────────────────
        public async Task<IActionResult> Warehouses()
        {
            var khos = await _ads.AdminGetKhoHangToWarehouses();
            return View(khos);
        }

        // ── THỐNG KÊ DOANH THU ───────────────────────────────────
        public async Task<IActionResult> Revenue()
        {
            var data = await _ads.AdminGetToRevenue();
            return View(data);
        }
    }
}
