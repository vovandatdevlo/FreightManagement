using FreightManagement.Data;
using FreightManagement.Models;
using FreightManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "QuanLyKho")]
    public class WarehouseController : Controller
    {
        private readonly WarehouseService _ws;
        public WarehouseController(WarehouseService ws) { _ws = ws; }

        private int GetUid() => int.Parse(User.FindFirst("UserId")!.Value);

        public async Task<IActionResult> Index()
        {
            ViewBag.ChoNhanVaoKho = await _ws.GetDonHangsCountByTrangThai("Đang xử lý");
            ViewBag.DangTrongKho = await _ws.GetDonHangsCountByTrangThai("Đã vào kho");
            ViewBag.DangVanChuyen = await _ws.GetDonHangsCountByTrangThai("Đang vận chuyển");
            ViewBag.DaGiao = await _ws.WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat("Đã giao");
            ViewBag.GiaoThatBai = await _ws.GetDonHangsCountByTrangThai("Giao thất bại");
            return View();
        }

        public async Task<IActionResult> Incoming()
        {
            var result = await _ws.IncomingService("Đang xử lý", GetUid());
            ViewBag.Khos = result.Warehouses;
            return View(result.orders);
        }

        [HttpPost]
        public async Task<IActionResult> NhanVaoKho(int maDon, int maKho)
        {
            var result = await _ws.NhanVaoKhoService(GetUid(), maDon, maKho);
            if (!result.IsValidItem) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Incoming");
        }

        public async Task<IActionResult> Assign()
        {
            var result = await _ws.AssignService(GetUid());
            ViewBag.TaiXeDict = result.Item1;
            return View(result.ordersList);
        }

        [HttpPost]
        public async Task<IActionResult> GanTaiXe(int maDon, int maTX)
        {
            var result = await _ws.GanTaiXeService(GetUid(), maDon, maTX);
            if (!result.valid) return NotFound();
            TempData["Success"] = result.message;
            return RedirectToAction("Assign");
        }

        // ── YC4: Danh sách đơn giao thất bại ─────────────────────────────
        public async Task<IActionResult> Failed()
        {
            var orders = await _ws.GetDonHangsGiaoThatBai();
            return View(orders);
        }

        // ── YC4: Xử lý giao lại ───────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> XuLyGiaoLai(int maDon)
        {
            var result = await _ws.XuLyGiaoLai(maDon, GetUid());
            if (!result.valid)
                TempData["Error"] = result.message;
            else
                TempData["Success"] = result.message;
            return RedirectToAction("Failed");
        }

        public async Task<IActionResult> Stock()
        {
            var uid = GetUid();  
            var items = await _ws.StockService(uid); 
            return View(items);
        }

        public async Task<IActionResult> Exported()
        {
            var uid = GetUid();  
            var items = await _ws.ExportedService(uid);  
            return View(items);
        }
    }
}