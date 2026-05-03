using System.ComponentModel.DataAnnotations;

namespace FreightManagement.DTOs
{
    public class AddStaffDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? SoDienThoai { get; set; }

        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [MaxLength(20)]
        public string? CCCD { get; set; }   // chỉ bắt buộc với TaiXe (validate ở Service)

        [Required(ErrorMessage = "Vui lòng chọn chức vụ")]
        [Range(3, 4, ErrorMessage = "Chức vụ không hợp lệ")]
        public int RoleId { get; set; }
    }
}
