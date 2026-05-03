using FreightManagement.Data;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Models;
using FreightManagement.DTOs;
using System.Runtime.CompilerServices;
using FreightManagement.MainRepositories.RepoInterfaces;

namespace FreightManagement.MainRepositories.Repository
{
    public class DonHangsRepository : IDonHangsRepository
    {
        private readonly AppDbContext _db;
        public DonHangsRepository(AppDbContext db)
        { 
            _db = db; 
        }

        public async Task UpdateOrder(DonHang order)
        {
            _db.DonHangs.Update(order);
            await _db.SaveChangesAsync();
        }

        public async Task<int> GetDonHangsCount()
        {
            return await _db.DonHangs.CountAsync();
        }

        public async Task<int> GetDonHangsCountByTrangThai(string trangthai)
        {
            return await _db.DonHangs.CountAsync(u => u.TrangThai == trangthai);
        }

        public async Task<List<DonHang>> GetRecentDonHangs()
        {
            return await _db.DonHangs
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayTao)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<DonHang>> GetFullOrders()
        {
            return await _db.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TaiXe)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();
        }

        // ── YC5: Filter + phân trang ─────────────────────────────────────────
        public async Task<(List<DonHang> items, int totalCount)> GetFullOrdersFiltered(
            string? trangThai, string? tuNgay, string? denNgay,
            string? tuKhoa, int page, int pageSize)
        {
            var query = _db.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TaiXe)
                .AsQueryable();

            // Lọc trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            // Lọc ngày tạo từ
            if (DateTime.TryParse(tuNgay, out var dateFrom))
                query = query.Where(d => d.NgayTao >= dateFrom);

            // Lọc ngày tạo đến (lấy hết ngày đó)
            if (DateTime.TryParse(denNgay, out var dateTo))
                query = query.Where(d => d.NgayTao < dateTo.AddDays(1));

            // Tìm kiếm từ khóa: MaDon | TenNguoiNhan | SdtNguoiNhan
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var kw = tuKhoa.Trim();
                if (int.TryParse(kw, out var maDon))
                    query = query.Where(d => d.MaDon == maDon
                        || d.TenNguoiNhan.Contains(kw)
                        || d.SdtNguoiNhan.Contains(kw));
                else
                    query = query.Where(d => d.TenNguoiNhan.Contains(kw)
                        || d.SdtNguoiNhan.Contains(kw));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(d => d.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        // ── YC4: Lấy theo trạng thái bất kỳ (include KhachHang, TaiXe) ──────
        public async Task<List<DonHang>> GetDonHangsByTrangThai(string trangthai)
        {
            return await _db.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TaiXe)
                .Where(d => d.TrangThai == trangthai)
                .OrderByDescending(d => d.NgayCapNhat)
                .ToListAsync();
        }

        public async Task<int> GetDonHangsCountByTrangThaiAndId(int uid, string trangthai)
        {
            return await _db.DonHangs.CountAsync(d => d.MaTX == uid && d.TrangThai == trangthai);
        }

        public async Task<List<DonHang>> TaixeGetMyDonHangs(int uid, string trangthai)
        {
            return await _db.DonHangs
                .Where(d => d.MaTX == uid && d.TrangThai == trangthai)
                .OrderBy(d => d.NgayCapNhat)
                .ToListAsync();
        }

        public async Task<DonHang> TaixeGetDonHangsByMadonAndMaTX(int madon, int MaTX)
        {
            return await _db.DonHangs
                .FirstOrDefaultAsync(d => d.MaDon == madon && d.MaTX == MaTX);
        }

        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThai(int MaTX, string trangthai)
        {
            return await _db.DonHangs
                .Where(d => d.MaTX == MaTX && d.TrangThai == trangthai)
                .ToListAsync();
        }

        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai)
        {
            return await _db.DonHangs
                .Where(d => d.MaTX == MaTX && d.TrangThai == trangthai)
                .OrderByDescending(d => d.NgayCapNhat)
                .ToListAsync();
        }

        public async Task<List<DonHang>> OrdersGetDonHangsByMaKHAndDescending(int uid)
        {
            return await _db.DonHangs
                .Include(d => d.TaiXe)
                .Where(d => d.MaKH == uid)
                .OrderByDescending(d => d.NgayTao)
                .ToListAsync();
        }

        public async Task OrdersAddDonHangs(DonHang order)
        {
            _db.DonHangs.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task<DonHang> OrdersGetDonHangsIncludeByMaDonAndMaKH(int id, int userId)
        {
            return await _db.DonHangs
                .Include(d => d.TaiXe)
                .Include(d => d.LichSuTrangThais)
                .Include(d => d.HangTrongKho).ThenInclude(h => h!.KhoHang)
                .FirstOrDefaultAsync(d => d.MaDon == id && d.MaKH == userId);
        }

        public async Task<DonHang> OrdersGetDonHangsByMaDonAndMaKH(int id, int userId)
        {
            return await _db.DonHangs
                .FirstOrDefaultAsync(d => d.MaDon == id && d.MaKH == userId);
        }

        public async Task<int> WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat(string trangthai)
        {
            return await _db.DonHangs
                .CountAsync(d => d.TrangThai == trangthai && d.NgayCapNhat.Date == DateTime.Today);
        }

        public async Task<List<DonHang>> WarehouseGetDonHangsToIncoming(string trangthai)
        {
            return await _db.DonHangs
                .Include(d => d.KhachHang)
                .Where(d => d.TrangThai == trangthai)
                .OrderBy(d => d.NgayTao)
                .ToListAsync();
        }

        public async Task<DonHang> WarehouseGetDonHangsToNhanVaoKho(int madon)
        {
            return await _db.DonHangs.FindAsync(madon);
        }

        public async Task<List<DonHang>> WarehouseGetDonHangsToAssignOrders(string trangthai)
        {
            return await _db.DonHangs
                .Include(d => d.KhachHang)
                .Include(d => d.TaiXe)
                .Include(d => d.HangTrongKho).ThenInclude(h => h!.KhoHang)
                .Where(d => d.TrangThai == trangthai)
                .ToListAsync();
        }

        public async Task<List<Taixetheokho>> WarehouseGetDonHangsToAssignTaixetheokho(string trangthai)
        {
            return await _db.DonHangs
                .Include(d => d.HangTrongKho).ThenInclude(h => h!.KhoHang)
                .Where(d => d.TrangThai == trangthai)
                .Select(d => new Taixetheokho
                {
                    MaDon = d.MaDon,
                    TinhKho = d.HangTrongKho!.KhoHang.DiaChiKho
                }).ToListAsync();
        }
    }
}