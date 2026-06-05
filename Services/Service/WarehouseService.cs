using FreightManagement.DTOs;
using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Repositories.Repository;
using FreightManagement.Models;
using FreightManagement.Services.IServices;

namespace FreightManagement.Services.Service
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IDonHangsRepository _OrdersRepo;
        private readonly IKhoHangsRepository _WareRepo;
        private readonly ILichSuTrangThaisRepository _hisRepo;
        private readonly IHangTrongKhosRepository _HTKRepo;
        private readonly IUsersRepository _UsersRepo;

        public WarehouseService(IDonHangsRepository OrdersRepo, IKhoHangsRepository WareRepo,
            ILichSuTrangThaisRepository hisRepo, IHangTrongKhosRepository HTKRepo, IUsersRepository UsersRepo)
        {
            _OrdersRepo = OrdersRepo;
            _WareRepo = WareRepo;
            _hisRepo = hisRepo;
            _HTKRepo = HTKRepo;
            _UsersRepo = UsersRepo;
        }

        public async Task<int> GetDonHangsCountByTrangThai(string trangthai, int MaQLK)
        {
            if (trangthai == "Đang xử lý")
            {
                var KhoHangList = await _WareRepo.WarehouseGetKhoHangToIncoming(MaQLK);
                var OrdersList = await _OrdersRepo.GetFullOrdersDangXuLy();
                int total = 0;
                for (int i = 0; i < KhoHangList.Count; i++)
                {
                    for (int j = 0; j < OrdersList.Count; j++)
                    {
                        if (KhoHangList[i].DiaChiKho == OrdersList[j].DiaChiGui)
                            total++;
                    }
                }
                return total;
            }
            return await _OrdersRepo.GetDonHangsCountByTrangThai(trangthai, MaQLK);
        }

        public async Task<int> WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat(string trangthai, int MaQLK)
        {
            return await _OrdersRepo.WarehouseGetDonHangsCountByTrangThaiAndNgayCapNhat(trangthai, MaQLK);
        }

        // Trường Incoming

        public async Task<(List<DonHang> orders, List<KhoHang> Warehouses)> IncomingService(string trangthai, int uid)
        {
            //var user = await _UsersRepo.GetUserById(uid);
            var tempOrders = await _OrdersRepo.WarehouseGetDonHangsToIncoming(trangthai);
            // THAY ĐỔI: Chỉ lấy kho của quản lý kho này
            var tempWarehouses = await _WareRepo.GetKhoHangByMaQLK(uid);
            var Orders = new List<DonHang>();
            for (int i = 0; i < tempOrders.Count; i++)
            {
                for (int j = 0; j < tempWarehouses.Count; j++)
                {
                    if (tempOrders[i].DiaChiGui == tempWarehouses[j].DiaChiKho)
                    {
                        Orders.Add(tempOrders[i]);
                    }
                }
            }
            return (Orders, tempWarehouses);
        }

        // Trường NhanVaoKho

        public async Task<(bool IsValidItem, string message)> NhanVaoKhoService(int uid, int madon, int makho)
        {
            var order = await _OrdersRepo.WarehouseGetDonHangsToNhanVaoKho(madon);
            var warehouse = await _WareRepo.WarehouseGetKhoHangToNhanVaoKho(makho);

            // Kiểm tra kho này có thuộc về quản lý kho này không
            if (warehouse == null || warehouse.MaQLK != uid)
                return (false, "Kho không tồn tại hoặc bạn không có quyền quản lý kho này.");

            if (order == null)
                return (false, "");

            order.TrangThai = "Đã vào kho";
            order.MaQLK = uid;
            await _OrdersRepo.UpdateOrder(order);

            await _HTKRepo.WarehouseAdd(new HangTrongKho { MaDon = madon, MaKho = makho });

            var khohang = await _WareRepo.GetKhoHangByDiaChiKho(order.DiaChiGui);

            if (order.DonGia == 25000 || order.DonGia == 50000)
                khohang.SoLuongHienTai += 1;

            else
                khohang.SoLuongHienTai += order.SoLuong;

            await _hisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = madon,
                TrangThai = "Đã vào kho",
                GhiChu = $"Nhập kho tại {warehouse.TenKho}",
                CapNhatBoi = uid,
            });
            return (true, $"Đã nhập đơn #DH{madon:D5} vào {warehouse.TenKho}!");
        }

        public async Task<List<KhoHang>> QuanLyKhoGetMyWarehouses(int uid)
        {
            return await _WareRepo.QuanLyKhoGetMyWarehouses(uid);
        }

        public async Task<(Dictionary<int, List<Users>>, List<DonHang> ordersList)> AssignService(int MaQLK)
        {
            var Warehouses = await _WareRepo.GetKhoHangByMaQLK(MaQLK);
            var orders = await _OrdersRepo.WarehouseGetDonHangsToAssignOrders("Đã vào kho");
            var taiXeTheoKho = await _OrdersRepo.WarehouseGetDonHangsToAssignTaixetheokho("Đã vào kho");
            var ordersList = new List<DonHang>();
            for (var i = 0; i < orders.Count; i++)
            {
                for (var j = 0; j < Warehouses.Count; j++)
                {
                    if (orders[i].DiaChiGui == Warehouses[j].DiaChiKho)
                    {
                        ordersList.Add(orders[i]);
                    }
                }
            }
            var taixeDict = new Dictionary<int, List<Users>>();
            foreach (var item in taiXeTheoKho)
            {
                var listTX = await _UsersRepo.GetTaixeByKho(item.TinhKho!);
                taixeDict[item.MaDon] = listTX;
            }
            return (taixeDict, ordersList);
        }

        // Trường GanTaixe

        public async Task<(bool valid, string message)> GanTaiXeService(int uid, int maDon, int maTX)
        {
            var order = await _OrdersRepo.WarehouseGetDonHangsToNhanVaoKho(maDon);
            var tx = await _UsersRepo.GetUserById(maTX);
            if (order == null || tx == null)
                return (false, "");

            order.MaTX = maTX;
            await _OrdersRepo.UpdateOrder(order);

            await _hisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Đã vào kho",
                GhiChu = $"Đã gán tài xế: {tx.HoTen}.",
                CapNhatBoi = uid,
            });
            return (true, $"Đã gán tài xế {tx.HoTen} cho đơn #DH{maDon:D5} thành công!");
        }

        public async Task<List<DonHang>> GetDonHangsGiaoThatBai()
        {
            return await _OrdersRepo.GetDonHangsByTrangThai("Giao thất bại");
        }

        public async Task<(bool valid, string message)> XuLyGiaoLai(int maDon, int uid)
        {
            var order = await _OrdersRepo.WarehouseGetDonHangsToNhanVaoKho(maDon);
            if (order == null || order.TrangThai != "Giao thất bại")
                return (false, "Không tìm thấy đơn hàng hoặc trạng thái không hợp lệ.");

            order.TrangThai = "Đã vào kho";
            await _OrdersRepo.UpdateOrder(order);
            await _hisRepo.DriverAddLichSuTrangThai(new LichSuTrangThai
            {
                MaDon = maDon,
                TrangThai = "Đã vào kho",
                GhiChu = "Quản lý kho xử lý giao lại, chờ gán tài xế mới",
                CapNhatBoi = uid,
            });
            return (true, $"Đơn #DH{maDon:D5} đã được đặt lại trạng thái chờ giao.");
        }

        public async Task<List<HangTrongKho>> StockService(int uid)
        {
            return await _HTKRepo.GetStockByMaQLK(uid);  
        }

        public async Task<List<HangTrongKho>> ExportedService(int uid)
        {
            return await _HTKRepo.GetExportedByMaQLK(uid);  
        }
    }
}