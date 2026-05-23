using FreightManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreightManagement.DTOs;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _ads;

        private static readonly List<string> TrangThaiList = new()
        {
            "Đang xử lý", "Đã vào kho", "Đang vận chuyển",
            "Đã giao", "Giao thất bại", "Đã hủy"
        };

        public AdminController(AdminService ads)
        {
            _ads = ads;
        }

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
            if (result.IsValidUser)
                TempData["Success"] = result.message;
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            ViewBag.Roles = new List<object>
            {
                new { RoleId = 3, Name = "Quản lý kho" },
                new { RoleId = 4, Name = "Tài xế" }
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStaff(AddStaffDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<object>
                {
                    new { RoleId = 3, Name = "Quản lý kho" },
                    new { RoleId = 4, Name = "Tài xế" }
                };
                return View(dto);
            }

            var result = await _ads.AddStaffService(dto);
            if (!result.success)
            {
                ViewBag.Error = result.message;
                ViewBag.Roles = new List<object>
                {
                    new { RoleId = 3, Name = "Quản lý kho" },
                    new { RoleId = 4, Name = "Tài xế" }
                };
                return View(dto);
            }

            TempData["Success"] = result.message;
            return RedirectToAction("Users");
        }

        [HttpGet]
        public async Task<IActionResult> Orders(
            string? trangThai, string? tuNgay, string? denNgay,
            string? tuKhoa, int page = 1)
        {
            const int pageSize = 20;
            var result = await _ads.GetFullOrdersFiltered(trangThai, tuNgay, denNgay, tuKhoa, page, pageSize);
            int totalPages = (int)Math.Ceiling(result.totalCount / (double)pageSize);

            ViewBag.TrangThaiList = TrangThaiList;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = result.totalCount;
            ViewBag.FilterTrangThai = trangThai;
            ViewBag.FilterTuNgay = tuNgay;
            ViewBag.FilterDenNgay = denNgay;
            ViewBag.FilterTuKhoa = tuKhoa;

            return View(result.items);
        }

        public async Task<IActionResult> Warehouses()
        {
            var khos = await _ads.AdminGetKhoHangToWarehouses();
            return View(khos);
        }

        // ── GÁN KHO CHO QUẢN LÝ KHO ────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> AssignWarehouse()
        {
            var khos = await _ads.AdminGetKhoHangToWarehouses();
            var qlks = await _ads.GetAllQuanLyKho();
            ViewBag.QuanLyKhos = qlks;
            return View(khos);
        }

        [HttpPost]
        public async Task<IActionResult> AssignWarehouse(int maKho, int maQLK)
        {
            var result = await _ads.AssignKhoToQuanLyKho(maKho, maQLK);
            if (result.success)
                TempData["Success"] = result.message;
            else
                TempData["Error"] = result.message;
            return RedirectToAction("AssignWarehouse");
        }

        public async Task<IActionResult> Revenue()
        {
            var data = await _ads.AdminGetToRevenue();
            return View(data);
        }

        // ── Cập nhật CCCD tài xế ────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateDriverCCCD(int driverId, string cccd)
        {
            var result = await _ads.UpdateDriverCCCD(driverId, cccd);
            if (result.success)
                TempData["Success"] = result.message;
            else
                TempData["Error"] = result.message;
            return RedirectToAction("Users");
        }
    }
}