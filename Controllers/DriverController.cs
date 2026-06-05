using FreightManagement.Data;
using FreightManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FreightManagement.Services.Service;
using FreightManagement.Services.IServices;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "TaiXe")]
    public class DriverController : Controller
    {
        private readonly IDriverService _ds;
        public DriverController(IDriverService ds) { _ds = ds; }

        private int GetUid() => int.Parse(User.FindFirst("UserId")!.Value);

        public async Task<IActionResult> Index()
        {
            var uid = GetUid();
            ViewBag.ChoXuLy = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã vào kho");
            ViewBag.DangGiao = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đang vận chuyển");
            ViewBag.HoanThanh = await _ds.GetDonHangsCountByTrangThaiAndId(uid, "Đã giao");
            return View();
        }

        public async Task<IActionResult> MyOrders()
        {
            var orders = await _ds.TaixeGetMyDonHangs(GetUid(), "Đã vào kho");
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> ChapNhan(int maDon)
        {
            var result = await _ds.ChapNhanService(GetUid(), maDon);
            if (!result.IsValidOrder) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("MyOrders");
        }

        public async Task<IActionResult> Delivering()
        {
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThai(GetUid(), "Đang vận chuyển");
            return View(orders);
        }

        // Trường

        [HttpPost]
        public async Task<IActionResult> GiaoThanhCong(int maDon)
        {
            var result = await _ds.GiaoThanhCongService(GetUid(), maDon);
            if (!result.IsValidOrder) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Delivering");
        }

        // ── YC4: Tài xế báo giao thất bại ────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> GiaoThatBai(int maDon)
        {
            var result = await _ds.GiaoThatBaiService(GetUid(), maDon);
            if (!result.IsValidOrder)
                TempData["Error"] = result.message;
            else
                TempData["Warning"] = result.message;
            return RedirectToAction("Delivering");
        }

        public async Task<IActionResult> Completed()
        {
            var orders = await _ds.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(GetUid(), "Đã giao");
            return View(orders);
        }
    }
}