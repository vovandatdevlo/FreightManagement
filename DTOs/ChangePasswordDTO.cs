namespace FreightManagement.DTOs
{
    public class ChangePasswordDTO
    {
        public string? currentPassword { get; set; }
        public string? newPassword { get; set; }
        public string? confirmPassword { get; set; }
    }
}
