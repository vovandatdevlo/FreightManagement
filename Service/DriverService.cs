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
            if (order == null)
                return (false, "");
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
            if (order == null)
                return (false, "");
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
        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai)
        {
            return await _OrdersRepo.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(MaTX, trangthai);
        }
    }
}
