using FreightManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _ads;
        public AdminController(AdminService ads) { _ads = ads; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TongDonHang = await _ads.GetDonHangsCount();
            ViewBag.DangXuLy = await _ads.GetDonHangsCountByTrangThai("Đang xử lý");
            ViewBag.DangVanChuyen = await _ads.GetDonHangsCountByTrangThai("Đang vận chuyển");
            ViewBag.DaGiao = await _ads.GetDonHangsCountByTrangThai("Đã giao");
            ViewBag.TongKhachHang = await _ads.GetUsersCountByRoleId(2);
            ViewBag.TongTaiXe = await _ads.GetUsersCountByRoleId(4);
            ViewBag.TongKho = await _ads.AdminGetTongKho();
            ViewBag.DoanhThu = await _ads.AdminGetSum();
            ViewBag.RecentOrders = await _ads.GetRecentDonHangs();
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _ads.GetUsersList();
            return View(users);
        }

        public async Task<IActionResult> ToggleUser(int id)
        {
            var result = await _ads.ToggleAccountService(id);
            if (result.IsValidUser) TempData["Success"] = result.message;
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _ads.GetFullOrders();
            return View(orders);
        }

        public async Task<IActionResult> Warehouses()
        {
            var khos = await _ads.AdminGetKhoHangToWarehouses();
            return View(khos);
        }

        public async Task<IActionResult> Revenue()
        {
            var data = await _ads.AdminGetToRevenue();
            return View(data);
        }
    }
}