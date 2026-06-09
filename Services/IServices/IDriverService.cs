using FreightManagement.Models;

namespace FreightManagement.Services.IServices
{
    public interface IDriverService
    {
        Task<int> GetDonHangsCountByTrangThaiAndId(int uid, string trangthai);
        Task<List<DonHang>> TaixeGetMyDonHangs(int uid, string trangthai);
        Task<(bool IsValidOrder, string message)> ChapNhanService(int uid, int maDon);
        Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThai(int MaTX, string trangthai);
        Task<(bool IsValidOrder, string message)> GiaoThanhCongService(int MaTX, int maDon);
        Task<(bool IsValidOrder, string message)> GiaoThatBaiService(int uid, int maDon, string lyDo);
        Task<List<DonHang>> TaixeGetDonHangsByMaTXAndTrangThaiAndDescending(int MaTX, string trangthai);
    }
}
