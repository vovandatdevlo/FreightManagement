using FreightManagement.DTOs;
using FreightManagement.Services.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FreightManagement.Services.IServices;

namespace FreightManagement.MainControllers
{
    [Authorize(Roles = "KhachHang")]
    public class OrdersController : Controller
    {
        private readonly IOrdersService _os;
        public OrdersController(IOrdersService os)
        {
            _os = os;
        }

        private static readonly List<string> Provinces = new()
        {
            "An Giang","Bà Rịa - Vũng Tàu","Bắc Giang","Bắc Kạn","Bạc Liêu","Bắc Ninh",
            "Bến Tre","Bình Định","Bình Dương","Bình Phước","Bình Thuận","Cà Mau","Cần Thơ",
            "Cao Bằng","Đà Nẵng","Đắk Lắk","Đắk Nông","Điện Biên","Đồng Nai","Đồng Tháp",
            "Gia Lai","Hà Giang","Hà Nam","Hà Nội","Hà Tĩnh","Hải Dương","Hải Phòng",
            "Hậu Giang","Hòa Bình","Hưng Yên","Khánh Hòa","Kiên Giang","Kon Tum","Lai Châu",
            "Lâm Đồng","Lạng Sơn","Lào Cai","Long An","Nam Định","Nghệ An","Ninh Bình",
            "Ninh Thuận","Phú Thọ","Phú Yên","Quảng Bình","Quảng Nam","Quảng Ngãi",
            "Quảng Ninh","Quảng Trị","Sóc Trăng","Sơn La","Tây Ninh","Thái Bình","Thái Nguyên",
            "Thanh Hóa","Thừa Thiên Huế","Tiền Giang","TP. Hồ Chí Minh","Trà Vinh","Tuyên Quang",
            "Vĩnh Long","Vĩnh Phúc","Yên Bái"
        };

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var orders = await _os.OrdersGetDonHangsByMaKHAndDescending(userId);
            return View(orders);
        }

        // Trường

        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var user = await _os.GetUserById(userId);
            ViewBag.HoTen = user?.HoTen;
            ViewBag.SoDienThoai = user?.SoDienThoai;
            ViewBag.DiaChi = user?.DiaChi;
            ViewBag.Provinces = Provinces;
            return View();
        }

        // Trường

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDTO obj)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _os.CreateOrderService(userId, obj);

            if (result.StartsWith("Số điện thoại"))
            {
                var user = await _os.GetUserById(userId);
                ViewBag.HoTen = user?.HoTen;
                ViewBag.SoDienThoai = user?.SoDienThoai;
                ViewBag.DiaChi = user?.DiaChi;
                ViewBag.Provinces = Provinces;
                ViewBag.Error = result;
                return View(obj);
            }

            TempData["Success"] = result;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _os.DetailOrderService(id, userId);
            if (!result.IsValidOrder) return NotFound();
            return View(result.order);
        }


        // Trường

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId")!.Value);
            var result = await _os.CancelService(id, userId);
            if (!result.IsValidOrder)
                TempData["Error"] = result.message;
            else
                TempData["Success"] = result.message;
            return RedirectToAction("Index");
        }
    }
}