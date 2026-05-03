using FreightManagement.Data;
using FreightManagement.Models;
using FreightManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "TaiXe")]
    public class DriverController : Controller
    {
        private readonly DriverService _ds;
        public DriverController(DriverService ds) 
        {
            _ds = ds; 
        }

        public async Task<IActionResult> Index()
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            ViewBag.ChoXuLy = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã vào kho");
            ViewBag.DangGiao = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đang vận chuyển");
            ViewBag.HoanThanh = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã giao");
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            var orders = await _ds.TaixeGetMyDonHangs(uid, "Đã vào kho");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChapNhan(int maDon)
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _ds.ChapNhanService(uid, maDon);
            if (!result.IsValidOrder) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> Delivering()
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThai(uid, "Đang vận chuyển");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> GiaoThanhCong(int maDon)
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _ds.GiaoThanhCongService(uid, maDon);
            if (!result.IsValidOrder) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Delivering");
        }

        public async Task<IActionResult> Completed()
        {
            var uid = int.Parse(User.FindFirst("UserId")!.Value);
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(uid, "Đã giao");
            return View(orders);
        }
    }
}