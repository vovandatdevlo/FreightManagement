using FreightManagement.Repositories;
using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using FreightManagement.Services.IServices;

namespace FreightManagement.Services.Service
{
    public class DriverService : IDriverService
    {
        private readonly IDonHangsRepository _OrdersRepo;
        private readonly ILichSuTrangThaisRepository _HisRepo;
        private readonly IThongKeDoanhThusRepository _TkRepo;
        private readonly IHangTrongKhosRepository _HTKRepo;
        private readonly IKhoHangsRepository _WareRepo;

        public DriverService(IDonHangsRepository OrdersRepo, ILichSuTrangThaisRepository HisRepo, IThongKeDoanhThusRepository TkRepo, IHangTrongKhosRepository HTKRepo, IKhoHangsRepository WareRepo)
        {
            _OrdersRepo = OrdersRepo;
            _HisRepo = HisRepo;
            _TkRepo = TkRepo;
            _HTKRepo = HTKRepo;
            _WareRepo = WareRepo;
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
            var HTK = await _HTKRepo.GetHangTrongKhoByMaDon(maDon);

            if (order == null) return (false, "");

            var khohang = await _WareRepo.GetKhoHangById(HTK!.MaKho);

            if (order.DonGia == 25000 || order.DonGia == 50000)
                khohang.SoLuongHienTai -= 1;

            else
                khohang.SoLuongHienTai -= order.SoLuong;

            order.TrangThai = "Đang vận chuyển";

            HTK!.ThoiGianXuatKho = DateTime.Now;

            await _HTKRepo.UpdateHangTrongKho(HTK);
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

            order.NgayCapNhat = DateTime.Now;

            await _OrdersRepo.UpdateOrder(order);
            await _TkRepo.AddThongKeDoanhThu(new ThongKeDoanhThu
            {
                Ngay = DateOnly.FromDateTime(DateTime.Now),
                MaDon = maDon,
                ChiPhiVanChuyen = order.ChiPhi,
            });
            await _HisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Đã giao",
                GhiChu = "Giao hàng thành công đến người nhận",
                CapNhatBoi = MaTX,
            });
            return (true, $"Đã giao thành công đơn #DH{maDon:D5}!");
        }

        // SAU
        public async Task<(bool IsValidOrder, string message)> GiaoThatBaiService(int uid, int maDon, string lyDo)
        {
            var order = await _OrdersRepo.TaixeGetDonHangsByMadonAndMaTX(maDon, uid);
            if (order == null || order.TrangThai != "Đang vận chuyển")
                return (false, "Không thể báo thất bại cho đơn hàng này.");

            // trả hàng về kho
            var htk = await _HTKRepo.GetHangTrongKhoByMaDon(maDon);
            if (htk != null)
            {
                var kho = await _WareRepo.GetKhoHangById(htk.MaKho);
                if (order.DonGia == 25000 || order.DonGia == 50000)
                    kho.SoLuongHienTai += 1;
                else
                    kho.SoLuongHienTai += order.SoLuong;

                htk.ThoiGianXuatKho = null; // hàng quay lại kho
                await _HTKRepo.UpdateHangTrongKho(htk);
            }

            order.TrangThai = "Đã vào kho";

            order.MaTX = null;

            await _OrdersRepo.UpdateOrder(order);
            await _HisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Giao thất bại",
                GhiChu = $"Giao thất bại: {lyDo}",
                CapNhatBoi = uid,
            });
            return (true, $"Đã ghi nhận giao thất bại đơn #DH{maDon:D5}. Hàng đã được trả về kho.");
        }
            //return (true, $"Đã ghi nhận giao thất bại đơn #DH{maDon:D5}. Quản lý kho sẽ xử lý tiếp.");


        public async Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai)
        {
            return await _OrdersRepo.TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(MaTX, trangthai);
        }
    }
}