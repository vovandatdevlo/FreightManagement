using FreightManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FreightManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<KhoHang> KhoHangs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<LichSuTrangThai> LichSuTrangThais { get; set; }
        public DbSet<HangTrongKho> HangTrongKhos { get; set; }
        public DbSet<ThongKeDoanhThu> ThongKeDoanhThus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<KhoHang>().ToTable("KhoHang");
            modelBuilder.Entity<DonHang>().ToTable("DonHang");
            modelBuilder.Entity<LichSuTrangThai>().ToTable("LichSuTrangThai");
            modelBuilder.Entity<HangTrongKho>().ToTable("HangTrongKho");
            modelBuilder.Entity<ThongKeDoanhThu>().ToTable("ThongKeDoanhThu");

            modelBuilder.Entity<DonHang>()
                .Property(d => d.ChiPhi)
                .HasComputedColumnSql("(SoLuong * DonGia) PERSISTED");

            // Composite PK
            modelBuilder.Entity<HangTrongKho>()
                .HasKey(h => new { h.MaKho, h.MaDon });

            // DonHang relationships
            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.KhachHang).WithMany(u => u.DonHangKhachHang)
                .HasForeignKey(d => d.MaKH).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.TaiXe).WithMany(u => u.DonHangTaiXe)
                .HasForeignKey(d => d.MaTX).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.QuanLyKho).WithMany(u => u.DonHangQuanLyKho)
                .HasForeignKey(d => d.MaQLK).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonHang>()
                .Property(d => d.ChiPhi).ValueGeneratedOnAddOrUpdate();

            // HangTrongKho relationships
            modelBuilder.Entity<HangTrongKho>()
                .HasOne(h => h.DonHang).WithOne(d => d.HangTrongKho)
                .HasForeignKey<HangTrongKho>(h => h.MaDon)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HangTrongKho>()
                .HasOne(h => h.KhoHang).WithMany(k => k.HangTrongKhos)
                .HasForeignKey(h => h.MaKho).OnDelete(DeleteBehavior.Restrict);

            // KhoHang relationships
            modelBuilder.Entity<KhoHang>()
                .HasOne(k => k.QuanLyKho).WithMany()
                .HasForeignKey(k => k.MaQLK).OnDelete(DeleteBehavior.Restrict);

            // LichSuTrangThai relationships
            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(l => l.DonHang).WithMany(d => d.LichSuTrangThais)
                .HasForeignKey(l => l.MaDon).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LichSuTrangThai>()
                .HasOne(l => l.NguoiCapNhat).WithMany()
                .HasForeignKey(l => l.CapNhatBoi).OnDelete(DeleteBehavior.Restrict);

            // ThongKeDoanhThu relationships
            modelBuilder.Entity<ThongKeDoanhThu>()
                .HasOne(t => t.DonHang).WithMany()
                .HasForeignKey(t => t.MaDon).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThongKeDoanhThu>()
                .HasIndex(t => t.MaDon).IsUnique();

            // Khai báo triggers để EF Core không dùng OUTPUT clause
            modelBuilder.Entity<DonHang>()
                .ToTable("DonHang", tb =>
                {
                    tb.HasTrigger("trg_LichSuTrangThai");
                    tb.HasTrigger("trg_ThongKeDoanhThu");
                    tb.HasTrigger("trg_HangXuatKho");
                    tb.HasTrigger("trg_TinhChiPhi");
                });

            modelBuilder.Entity<HangTrongKho>()
                .ToTable("HangTrongKho", tb =>
                {
                    tb.HasTrigger("trg_HangVaoKho");
                });
            // ========================
            // SEED DATA (giống dbvanchuyen2.sql)
            // ========================

            // 1. Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "KhachHang" },
                new Role { RoleId = 3, RoleName = "QuanLyKho" },
                new Role { RoleId = 4, RoleName = "TaiXe" }
            );

            // 2. Users (password hash giữ nguyên như sql)
            modelBuilder.Entity<Users>().HasData(
                new Users { UserId = 1, HoTen = "Admin Hệ Thống", Email = "admin@vanchuyen.com", PasswordHash = "HASH_password123", SoDienThoai = "0900000001", DiaChi = "Hà Nội", RoleId = 1, TrangThai = "HoatDong", NgayTao = DateTime.Now },
                new Users { UserId = 2, HoTen = "Nguyễn Văn A", Email = "khachhang@gmail.com", PasswordHash = "HASH_password123", SoDienThoai = "0912345678", DiaChi = "TP.HCM", RoleId = 2, TrangThai = "HoatDong", NgayTao = DateTime.Now },
                new Users { UserId = 3, HoTen = "Trần Thị B", Email = "quanlykho@gmail.com", PasswordHash = "HASH_password123", SoDienThoai = "0987654321", DiaChi = "Đà Nẵng", RoleId = 3, TrangThai = "HoatDong", NgayTao = DateTime.Now },
                new Users { UserId = 4, HoTen = "Lê Văn C", Email = "taixe@gmail.com", PasswordHash = "HASH_password123", SoDienThoai = "0976543210", DiaChi = "Hà Nội", RoleId = 4, TrangThai = "HoatDong", NgayTao = DateTime.Now },
                new Users { UserId = 5, HoTen = "Lê Văn D", Email = "taixe1@gmail.com", PasswordHash = "HASH_password123", SoDienThoai = "0976543211", DiaChi = "Hà Nội", RoleId = 4, TrangThai = "HoatDong", NgayTao = DateTime.Now }
            );

            // 3. KhoHang (tất cả 63 kho từ file sql)
            modelBuilder.Entity<KhoHang>().HasData(
                new KhoHang { MaKho = 1, TenKho = "Kho Hà Nội", DiaChiKho = "Hà Nội", SucChua = 500, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 2, TenKho = "Kho TP.HCM", DiaChiKho = "TP.HCM", SucChua = 800, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 3, TenKho = "Kho Hải Phòng", DiaChiKho = "Hải Phòng", SucChua = 400, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 4, TenKho = "Kho Đà Nẵng", DiaChiKho = "Đà Nẵng", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 5, TenKho = "Kho Cần Thơ", DiaChiKho = "Cần Thơ", SucChua = 350, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 6, TenKho = "Kho An Giang", DiaChiKho = "An Giang", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 7, TenKho = "Kho BR - VT", DiaChiKho = "Bà Rịa - Vũng Tàu", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 8, TenKho = "Kho Bắc Giang", DiaChiKho = "Bắc Giang", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 9, TenKho = "Kho Bắc Kạn", DiaChiKho = "Bắc Kạn", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 10, TenKho = "Kho Bạc Liêu", DiaChiKho = "Bạc Liêu", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 11, TenKho = "Kho Bắc Ninh", DiaChiKho = "Bắc Ninh", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 12, TenKho = "Kho Bến Tre", DiaChiKho = "Bến Tre", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 13, TenKho = "Kho Bình Định", DiaChiKho = "Bình Định", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 14, TenKho = "Kho Bình Dương", DiaChiKho = "Bình Dương", SucChua = 500, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 15, TenKho = "Kho Bình Phước", DiaChiKho = "Bình Phước", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 16, TenKho = "Kho Bình Thuận", DiaChiKho = "Bình Thuận", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 17, TenKho = "Kho Cà Mau", DiaChiKho = "Cà Mau", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 18, TenKho = "Kho Cao Bằng", DiaChiKho = "Cao Bằng", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 19, TenKho = "Kho Đắk Lắk", DiaChiKho = "Đắk Lắk", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 20, TenKho = "Kho Đắk Nông", DiaChiKho = "Đắk Nông", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 21, TenKho = "Kho Điện Biên", DiaChiKho = "Điện Biên", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 22, TenKho = "Kho Đồng Nai", DiaChiKho = "Đồng Nai", SucChua = 500, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 23, TenKho = "Kho Đồng Tháp", DiaChiKho = "Đồng Tháp", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 24, TenKho = "Kho Gia Lai", DiaChiKho = "Gia Lai", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 25, TenKho = "Kho Hà Giang", DiaChiKho = "Hà Giang", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 26, TenKho = "Kho Hà Nam", DiaChiKho = "Hà Nam", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 27, TenKho = "Kho Hà Tĩnh", DiaChiKho = "Hà Tĩnh", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 28, TenKho = "Kho Hải Dương", DiaChiKho = "Hải Dương", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 29, TenKho = "Kho Hậu Giang", DiaChiKho = "Hậu Giang", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 30, TenKho = "Kho Hòa Bình", DiaChiKho = "Hòa Bình", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 31, TenKho = "Kho Hưng Yên", DiaChiKho = "Hưng Yên", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 32, TenKho = "Kho Khánh Hòa", DiaChiKho = "Khánh Hòa", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 33, TenKho = "Kho Kiên Giang", DiaChiKho = "Kiên Giang", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 34, TenKho = "Kho Kon Tum", DiaChiKho = "Kon Tum", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 35, TenKho = "Kho Lai Châu", DiaChiKho = "Lai Châu", SucChua = 200, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 36, TenKho = "Kho Lâm Đồng", DiaChiKho = "Lâm Đồng", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 37, TenKho = "Kho Lạng Sơn", DiaChiKho = "Lạng Sơn", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 38, TenKho = "Kho Lào Cai", DiaChiKho = "Lào Cai", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 39, TenKho = "Kho Long An", DiaChiKho = "Long An", SucChua = 350, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 40, TenKho = "Kho Nam Định", DiaChiKho = "Nam Định", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 41, TenKho = "Kho Nghệ An", DiaChiKho = "Nghệ An", SucChua = 400, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 42, TenKho = "Kho Ninh Bình", DiaChiKho = "Ninh Bình", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 43, TenKho = "Kho Ninh Thuận", DiaChiKho = "Ninh Thuận", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 44, TenKho = "Kho Phú Thọ", DiaChiKho = "Phú Thọ", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 45, TenKho = "Kho Phú Yên", DiaChiKho = "Phú Yên", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 46, TenKho = "Kho Quảng Bình", DiaChiKho = "Quảng Bình", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 47, TenKho = "Kho Quảng Nam", DiaChiKho = "Quảng Nam", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 48, TenKho = "Kho Quảng Ngãi", DiaChiKho = "Quảng Ngãi", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 49, TenKho = "Kho Quảng Ninh", DiaChiKho = "Quảng Ninh", SucChua = 400, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 50, TenKho = "Kho Quảng Trị", DiaChiKho = "Quảng Trị", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 51, TenKho = "Kho Sóc Trăng", DiaChiKho = "Sóc Trăng", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 52, TenKho = "Kho Sơn La", DiaChiKho = "Sơn La", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 53, TenKho = "Kho Tây Ninh", DiaChiKho = "Tây Ninh", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 54, TenKho = "Kho Thái Bình", DiaChiKho = "Thái Bình", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 55, TenKho = "Kho Thái Nguyên", DiaChiKho = "Thái Nguyên", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 56, TenKho = "Kho Thanh Hóa", DiaChiKho = "Thanh Hóa", SucChua = 400, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 57, TenKho = "Kho Thừa Thiên Huế", DiaChiKho = "Thừa Thiên Huế", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 58, TenKho = "Kho Tiền Giang", DiaChiKho = "Tiền Giang", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 59, TenKho = "Kho Trà Vinh", DiaChiKho = "Trà Vinh", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 60, TenKho = "Kho Tuyên Quang", DiaChiKho = "Tuyên Quang", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 61, TenKho = "Kho Vĩnh Long", DiaChiKho = "Vĩnh Long", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 62, TenKho = "Kho Vĩnh Phúc", DiaChiKho = "Vĩnh Phúc", SucChua = 300, SoLuongHienTai = 0, MaQLK = 3 },
                new KhoHang { MaKho = 63, TenKho = "Kho Yên Bái", DiaChiKho = "Yên Bái", SucChua = 250, SoLuongHienTai = 0, MaQLK = 3 }
            );
        }   
    }
}
