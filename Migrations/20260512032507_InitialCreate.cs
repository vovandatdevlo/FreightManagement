using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FreightManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaTX = table.Column<int>(type: "int", nullable: true),
                    MaQLK = table.Column<int>(type: "int", nullable: true),
                    DiaChiGui = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChiNhan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SdtNguoiNhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MoTaHang = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<int>(type: "int", nullable: false),
                    ChiPhi = table.Column<int>(type: "int", nullable: false, computedColumnSql: "(SoLuong * DonGia) PERSISTED"),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.MaDon);
                    table.ForeignKey(
                        name: "FK_DonHang_Users_MaKH",
                        column: x => x.MaKH,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHang_Users_MaQLK",
                        column: x => x.MaQLK,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonHang_Users_MaTX",
                        column: x => x.MaTX,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhoHang",
                columns: table => new
                {
                    MaKho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKho = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChiKho = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SucChua = table.Column<int>(type: "int", nullable: false),
                    SoLuongHienTai = table.Column<int>(type: "int", nullable: false),
                    MaQLK = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoHang", x => x.MaKho);
                    table.ForeignKey(
                        name: "FK_KhoHang_Users_MaQLK",
                        column: x => x.MaQLK,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuTrangThai",
                columns: table => new
                {
                    MaLS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDon = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CapNhatBoi = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuTrangThai", x => x.MaLS);
                    table.ForeignKey(
                        name: "FK_LichSuTrangThai_DonHang_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonHang",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuTrangThai_Users_CapNhatBoi",
                        column: x => x.CapNhatBoi,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeDoanhThu",
                columns: table => new
                {
                    MaThongKe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ngay = table.Column<DateOnly>(type: "date", nullable: false),
                    MaDon = table.Column<int>(type: "int", nullable: false),
                    ChiPhiVanChuyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeDoanhThu", x => x.MaThongKe);
                    table.ForeignKey(
                        name: "FK_ThongKeDoanhThu_DonHang_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonHang",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HangTrongKho",
                columns: table => new
                {
                    MaKho = table.Column<int>(type: "int", nullable: false),
                    MaDon = table.Column<int>(type: "int", nullable: false),
                    ThoiGianVaoKho = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianXuatKho = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangTrongKho", x => new { x.MaKho, x.MaDon });
                    table.ForeignKey(
                        name: "FK_HangTrongKho_DonHang_MaDon",
                        column: x => x.MaDon,
                        principalTable: "DonHang",
                        principalColumn: "MaDon",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HangTrongKho_KhoHang_MaKho",
                        column: x => x.MaKho,
                        principalTable: "KhoHang",
                        principalColumn: "MaKho",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "KhachHang" },
                    { 3, "QuanLyKho" },
                    { 4, "TaiXe" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CCCD", "DiaChi", "Email", "HoTen", "NgayTao", "PasswordHash", "RoleId", "SoDienThoai", "TrangThai" },
                values: new object[,]
                {
                    { 1, null, "Hà Nội", "admin@vanchuyen.com", "Admin Hệ Thống", new DateTime(2026, 5, 12, 10, 25, 7, 681, DateTimeKind.Local).AddTicks(5318), "HASH_password123", 1, "0900000001", "HoatDong" },
                    { 2, null, "TP.HCM", "khachhang@gmail.com", "Nguyễn Văn A", new DateTime(2026, 5, 12, 10, 25, 7, 681, DateTimeKind.Local).AddTicks(5321), "HASH_password123", 2, "0912345678", "HoatDong" },
                    { 3, null, "Đà Nẵng", "quanlykho@gmail.com", "Trần Thị B", new DateTime(2026, 5, 12, 10, 25, 7, 681, DateTimeKind.Local).AddTicks(5323), "HASH_password123", 3, "0987654321", "HoatDong" },
                    { 4, null, "Hà Nội", "taixe@gmail.com", "Lê Văn C", new DateTime(2026, 5, 12, 10, 25, 7, 681, DateTimeKind.Local).AddTicks(5325), "HASH_password123", 4, "0976543210", "HoatDong" },
                    { 5, null, "Hà Nội", "taixe1@gmail.com", "Lê Văn D", new DateTime(2026, 5, 12, 10, 25, 7, 681, DateTimeKind.Local).AddTicks(5327), "HASH_password123", 4, "0976543211", "HoatDong" }
                });

            migrationBuilder.InsertData(
                table: "KhoHang",
                columns: new[] { "MaKho", "DiaChiKho", "MaQLK", "SoLuongHienTai", "SucChua", "TenKho" },
                values: new object[,]
                {
                    { 1, "Hà Nội", 3, 0, 500, "Kho Hà Nội" },
                    { 2, "TP.HCM", 3, 0, 800, "Kho TP.HCM" },
                    { 3, "Hải Phòng", 3, 0, 400, "Kho Hải Phòng" },
                    { 4, "Đà Nẵng", 3, 0, 300, "Kho Đà Nẵng" },
                    { 5, "Cần Thơ", 3, 0, 350, "Kho Cần Thơ" },
                    { 6, "An Giang", 3, 0, 300, "Kho An Giang" },
                    { 7, "Bà Rịa - Vũng Tàu", 3, 0, 300, "Kho BR - VT" },
                    { 8, "Bắc Giang", 3, 0, 300, "Kho Bắc Giang" },
                    { 9, "Bắc Kạn", 3, 0, 200, "Kho Bắc Kạn" },
                    { 10, "Bạc Liêu", 3, 0, 250, "Kho Bạc Liêu" },
                    { 11, "Bắc Ninh", 3, 0, 300, "Kho Bắc Ninh" },
                    { 12, "Bến Tre", 3, 0, 250, "Kho Bến Tre" },
                    { 13, "Bình Định", 3, 0, 300, "Kho Bình Định" },
                    { 14, "Bình Dương", 3, 0, 500, "Kho Bình Dương" },
                    { 15, "Bình Phước", 3, 0, 300, "Kho Bình Phước" },
                    { 16, "Bình Thuận", 3, 0, 300, "Kho Bình Thuận" },
                    { 17, "Cà Mau", 3, 0, 250, "Kho Cà Mau" },
                    { 18, "Cao Bằng", 3, 0, 200, "Kho Cao Bằng" },
                    { 19, "Đắk Lắk", 3, 0, 300, "Kho Đắk Lắk" },
                    { 20, "Đắk Nông", 3, 0, 250, "Kho Đắk Nông" },
                    { 21, "Điện Biên", 3, 0, 200, "Kho Điện Biên" },
                    { 22, "Đồng Nai", 3, 0, 500, "Kho Đồng Nai" },
                    { 23, "Đồng Tháp", 3, 0, 300, "Kho Đồng Tháp" },
                    { 24, "Gia Lai", 3, 0, 300, "Kho Gia Lai" },
                    { 25, "Hà Giang", 3, 0, 200, "Kho Hà Giang" },
                    { 26, "Hà Nam", 3, 0, 250, "Kho Hà Nam" },
                    { 27, "Hà Tĩnh", 3, 0, 250, "Kho Hà Tĩnh" },
                    { 28, "Hải Dương", 3, 0, 300, "Kho Hải Dương" },
                    { 29, "Hậu Giang", 3, 0, 250, "Kho Hậu Giang" },
                    { 30, "Hòa Bình", 3, 0, 250, "Kho Hòa Bình" },
                    { 31, "Hưng Yên", 3, 0, 300, "Kho Hưng Yên" },
                    { 32, "Khánh Hòa", 3, 0, 300, "Kho Khánh Hòa" },
                    { 33, "Kiên Giang", 3, 0, 300, "Kho Kiên Giang" },
                    { 34, "Kon Tum", 3, 0, 200, "Kho Kon Tum" },
                    { 35, "Lai Châu", 3, 0, 200, "Kho Lai Châu" },
                    { 36, "Lâm Đồng", 3, 0, 300, "Kho Lâm Đồng" },
                    { 37, "Lạng Sơn", 3, 0, 250, "Kho Lạng Sơn" },
                    { 38, "Lào Cai", 3, 0, 250, "Kho Lào Cai" },
                    { 39, "Long An", 3, 0, 350, "Kho Long An" },
                    { 40, "Nam Định", 3, 0, 300, "Kho Nam Định" },
                    { 41, "Nghệ An", 3, 0, 400, "Kho Nghệ An" },
                    { 42, "Ninh Bình", 3, 0, 250, "Kho Ninh Bình" },
                    { 43, "Ninh Thuận", 3, 0, 250, "Kho Ninh Thuận" },
                    { 44, "Phú Thọ", 3, 0, 300, "Kho Phú Thọ" },
                    { 45, "Phú Yên", 3, 0, 250, "Kho Phú Yên" },
                    { 46, "Quảng Bình", 3, 0, 250, "Kho Quảng Bình" },
                    { 47, "Quảng Nam", 3, 0, 300, "Kho Quảng Nam" },
                    { 48, "Quảng Ngãi", 3, 0, 300, "Kho Quảng Ngãi" },
                    { 49, "Quảng Ninh", 3, 0, 400, "Kho Quảng Ninh" },
                    { 50, "Quảng Trị", 3, 0, 250, "Kho Quảng Trị" },
                    { 51, "Sóc Trăng", 3, 0, 250, "Kho Sóc Trăng" },
                    { 52, "Sơn La", 3, 0, 250, "Kho Sơn La" },
                    { 53, "Tây Ninh", 3, 0, 300, "Kho Tây Ninh" },
                    { 54, "Thái Bình", 3, 0, 300, "Kho Thái Bình" },
                    { 55, "Thái Nguyên", 3, 0, 300, "Kho Thái Nguyên" },
                    { 56, "Thanh Hóa", 3, 0, 400, "Kho Thanh Hóa" },
                    { 57, "Thừa Thiên Huế", 3, 0, 300, "Kho Thừa Thiên Huế" },
                    { 58, "Tiền Giang", 3, 0, 300, "Kho Tiền Giang" },
                    { 59, "Trà Vinh", 3, 0, 250, "Kho Trà Vinh" },
                    { 60, "Tuyên Quang", 3, 0, 250, "Kho Tuyên Quang" },
                    { 61, "Vĩnh Long", 3, 0, 250, "Kho Vĩnh Long" },
                    { 62, "Vĩnh Phúc", 3, 0, 300, "Kho Vĩnh Phúc" },
                    { 63, "Yên Bái", 3, 0, 250, "Kho Yên Bái" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKH",
                table: "DonHang",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaQLK",
                table: "DonHang",
                column: "MaQLK");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaTX",
                table: "DonHang",
                column: "MaTX");

            migrationBuilder.CreateIndex(
                name: "IX_HangTrongKho_MaDon",
                table: "HangTrongKho",
                column: "MaDon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhoHang_MaQLK",
                table: "KhoHang",
                column: "MaQLK");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTrangThai_CapNhatBoi",
                table: "LichSuTrangThai",
                column: "CapNhatBoi");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuTrangThai_MaDon",
                table: "LichSuTrangThai",
                column: "MaDon");

            migrationBuilder.CreateIndex(
                name: "IX_ThongKeDoanhThu_MaDon",
                table: "ThongKeDoanhThu",
                column: "MaDon",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HangTrongKho");

            migrationBuilder.DropTable(
                name: "LichSuTrangThai");

            migrationBuilder.DropTable(
                name: "ThongKeDoanhThu");

            migrationBuilder.DropTable(
                name: "KhoHang");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
