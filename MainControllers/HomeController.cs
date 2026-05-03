using System.Diagnostics;
using FreightManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace FreightManagement.MainControllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity!.IsAuthenticated) return View();

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "KhachHang" => RedirectToAction("Index", "Orders"),
                "QuanLyKho" => RedirectToAction("Index", "Warehouse"),
                "TaiXe" => RedirectToAction("Index", "Driver"),
                _ => View()
            };
        }
    }
}
