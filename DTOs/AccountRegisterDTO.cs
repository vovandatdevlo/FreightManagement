namespace FreightManagement.DTOs
{
    public class AccountRegisterDTO
    {
        public string hoTen { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }
        public string? soDienThoai { get; set; }
        public string? diaChi { get; set; }
    }
}
