namespace FreightManagement.DTOs
{
    public class CreateOrderDTO
    {
        public string? TenNguoiNhan { get; set;}
        public string? SdtNguoiNhan { get; set; }
        public string? TinhNhan { get; set; }
        public string? DiaChiNhan { get; set; }
        public string? MoTaHang { get; set; }
        public int SoLuong { get; set; }
        public string? LoaiHang { get; set; }
    }
}
