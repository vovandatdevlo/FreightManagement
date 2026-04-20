using FreightManagement.Data;
using FreightManagement.Filters;
using FreightManagement.Models;
using FreightManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreightManagement.MainControllers
{
    [RequireLogin("TaiXe")]
    public class DriverController : Controller
    {
        private readonly DriverService _ds;
        public DriverController(DriverService ds)
        {
            _ds = ds;
        }

        public async Task<IActionResult> Index()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            ViewBag.ChoXuLy = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã vào kho");
            ViewBag.DangGiao = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đang vận chuyển");
            ViewBag.HoanThanh = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã giao");
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var orders = await _ds.TaixeGetMyDonHangs(uid, "Đã vào kho");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChapNhan(int maDon)
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _ds.ChapNhanService(uid, maDon);
            if (!result.IsValidOrder)
                return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> Delivering()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThai(uid, "Đang vận chuyển");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> GiaoThanhCong(int maDon)
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _ds.GiaoThanhCongService(uid, maDon);
            if (!result.IsValidOrder)
                return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Delivering");
        }

        public async Task<IActionResult> Completed()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(uid, "Đã giao");
            return View(orders);
        }
    }
}
