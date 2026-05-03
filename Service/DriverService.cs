using FreightManagement.MainRepositories;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FreightManagement.Service
{
    public class DriverService
    {
        private readonly IDonHangsRepository _OrdersRepo;
        private readonly ILichSuTrangThaisRepository _HisRepo;

        public DriverService(IDonHangsRepository OrdersRepo, ILichSuTrangThaisRepository HisRepo)
        {
            _OrdersRepo = OrdersRepo;
            _HisRepo = HisRepo;
        }

        public async Task<int> GetDonHangsCountByTrangThaiAndId(int uid, string trangthai)
        {
            return await _OrdersRepo.GetDonHangsCountByTrangThaiAndId(uid, trangthai);
        }

        public async Task<List<DonHang>> TaixeGetMyDonHangs(int uid, string trangthai)
        {
            return await _OrdersRepo.TaixeGetMyDonHangs(uid, trangthai);
        }

        public async Task<(bool IsValidOrder, string message)> ChapNhanService(int uid, int maDon)
        {
            var order = await _OrdersRepo.TaixeGetDonHangsByMadonAndMaTX(maDon, uid);
            if (order == null) return (false, "");

            order.TrangThai = "Đang vận chuyển";
            await _OrdersRepo.UpdateOrder(order);
            await _HisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Đang vận chuyển",
                GhiChu = "Tài xế đã nhận hàng và bắt đầu vận chuyển",
                CapNhatBoi = uid,
            });
            return (true, $"Đã xác nhận nhận đơn #DH{maDon:D5}, bắt đầu vận chuyển!");
        }

        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThai(int MaTX, string trangthai)
        {
            return await _OrdersRepo.TaixeGetDonHangsByMaTXAndTrangThai(MaTX, trangthai);
        }

        public async Task<(bool IsValidOrder, string message)> GiaoThanhCongService(int MaTX, int maDon)
        {
            var order = await _OrdersRepo.TaixeGetDonHangsByMadonAndMaTX(maDon, MaTX);
            if (order == null) return (false, "");

            order.TrangThai = "Đã giao";
            await _OrdersRepo.UpdateOrder(order);
            await _HisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Đã giao",
                GhiChu = "Giao hàng thành công đến người nhận",
                CapNhatBoi = MaTX,
            });
            return (true, $"Đã giao thành công đơn #DH{maDon:D5}!");
        }

        // ── YC4: Tài xế báo giao thất bại ───────────────────────────────────
        public async Task<(bool IsValidOrder, string message)> GiaoThatBaiService(int uid, int maDon)
        {
            var order = await _OrdersRepo.TaixeGetDonHangsByMadonAndMaTX(maDon, uid);
            if (order == null || order.TrangThai != "Đang vận chuyển")
                return (false, "Không thể báo thất bại cho đơn hàng này.");

            order.TrangThai = "Giao thất bại";
            await _OrdersRepo.UpdateOrder(order);
            await _HisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Giao thất bại",
                GhiChu = "Tài xế báo giao thất bại",
                CapNhatBoi = uid,
            });
            return (true, $"Đã ghi nhận giao thất bại đơn #DH{maDon:D5}. Quản lý kho sẽ xử lý tiếp.");
        }

        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai)
        {
            return await _OrdersRepo.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(MaTX, trangthai);
        }
    }
}