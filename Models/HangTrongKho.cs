using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreightManagement.Models
{
    public class HangTrongKho
    {
        //[Key, Column(Order = 0)]
        public int MaKho { get; set; }
        //[Key, Column(Order = 1)]
        public int MaDon { get; set; }
        public DateTime ThoiGianVaoKho { get; set; } = DateTime.Now;
        public DateTime? ThoiGianXuatKho { get; set; }

        // Navigation
        public KhoHang KhoHang { get; set; } = null!;
        public DonHang DonHang { get; set; } = null!;
    }
}
