using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Services
{
    public interface IPhieuSuaChuaService
    {
        Task<List<SanPham>> GetUserProducts(string userId);
        Task<(bool success, string message)> CreatePhieuSuaChua(PhieuSuaChuaVM model, string userId);
    }
}
