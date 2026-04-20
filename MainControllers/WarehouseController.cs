using FreightManagement.Data;
using FreightManagement.Filters;
using FreightManagement.Models;
using FreightManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreightManagement.MainControllers
{
    [RequireLogin("QuanLyKho")]
    public class WarehouseController : Controller
    {
        private readonly WarehouseService _ws;
        public WarehouseController(WarehouseService ws)
        {
            _ws = ws;
        }

        public async Task<IActionResult> Index()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            ViewBag.ChoNhanVaoKho = await _ws.GetDonHangsCountByTrangThai("Đang xử lý");
            ViewBag.DangTrongKho = await _ws.GetDonHangsCountByTrangThai("Đã vào kho");
            ViewBag.DangVanChuyen = await _ws.GetDonHangsCountByTrangThai("Đang vận chuyển");
            ViewBag.DaGiao = await _ws.WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat("Đã giao");
            return View();
        }

        public async Task<IActionResult> Incoming()
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _ws.IncomingService("Đang xử lý", uid);
            ViewBag.Khos = result.Warehouses;
            return View(result.orders);
        }

        [HttpPost]
        public async Task<IActionResult> NhanVaoKho(int maDon, int maKho)
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _ws.NhanVaoKhoService(uid, maDon, maKho);
            if (!result.IsValidItem)
                return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Incoming");
        }

        public async Task<IActionResult> Assign()
        {
            var result = await _ws.AssignService();
            ViewBag.TaiXeDict = result.Item1;
            return View(result.orders);
        }

        [HttpPost]
        public async Task<IActionResult> GanTaiXe(int maDon, int maTX)
        {
            var uid = HttpContext.Session.GetInt32("UserId")!.Value;
            var result = await _ws.GanTaiXeService(uid, maDon, maTX);
            if (!result.valid)
                return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Assign");
        }

        public async Task<IActionResult> Stock()
        {
            var items = await _ws.StockService();
            return View(items);
        }

        public async Task<IActionResult> Exported()
        {
            var items = await _ws.ExportedService();
            return View(items);
        }
    }
}
