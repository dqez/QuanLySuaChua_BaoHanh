using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Services
{
    public class PhieuSuaChuaService : IPhieuSuaChuaService
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;
        private readonly MapService _mapService;
        public PhieuSuaChuaService(BHSC_DbContext context, IDGenerator idGenerator, MapService mapService)
        {
            _context = context;
            _idGenerator = idGenerator;
            _mapService = mapService;
        }

        public async Task<List<SanPham>> GetUserProducts(string userId)
        {
            return await _context.SanPhams
                .Where(p => p.KhachHangId == userId)
                .ToListAsync();
        }

        public async Task<(bool success, string message)> CreatePhieuSuaChua(PhieuSuaChuaVM model, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {                
                var phieuSuaChua = new PhieuSuaChua
                {
                    PhieuSuaChuaId = await _idGenerator.GenerateIdAsync_Date("PSC"),
                    KhachHangId = userId,
                    NgayGui = DateTime.Now,
                    TrangThai = QuanLySuaChua_BaoHanh.Enums.TrangThaiPhieu.ChoXacNhan.ToString(),
                    MoTaKhachHang = model.MoTaKhachHang,
                    DiaChiNhanTraSanPham = model.DiaChiNhanTraSanPham,
                    PhuongId = model.PhuongId
                };

                // Lấy thông tin địa chỉ đầy đủ
                var diaChiInfo = await _context.Phuongs
                    .Include(p => p.Quan)
                        .ThenInclude(q => q.ThanhPho)
                    .Where(p => p.PhuongId == model.PhuongId)
                    .Select(p => new {
                        TenPhuong = p.TenPhuong, 
                        TenQuan = p.Quan.TenQuan,
                        TenThanhPho = p.Quan.ThanhPho.TenThanhPho
                    })
                    .FirstOrDefaultAsync();
                string diachi = null;
                if (diaChiInfo != null)
                {
                    diachi = $"{model.DiaChiNhanTraSanPham}, {diaChiInfo.TenPhuong}, {diaChiInfo.TenQuan}, {diaChiInfo.TenThanhPho}";
                }

                var start = await _mapService.GeocodeAsync("02 Thanh Sơn, Thanh Bình, Hải Châu, Đà Nẵng");
                var end = await _mapService.GeocodeAsync(diachi);
                var route = await _mapService.GetRouteAsync(start, end);

                phieuSuaChua.KhoangCach = route.Distance;
                phieuSuaChua.PhiVanChuyen = 50000; // Giả sử phí vận chuyển là 15.000 VNĐ
                
                phieuSuaChua.TongTien = phieuSuaChua.PhiVanChuyen *
                         (phieuSuaChua.KhoangCach != null ?
                          (decimal)phieuSuaChua.KhoangCach.Value : 0m);

                await _context.PhieuSuaChuas.AddAsync(phieuSuaChua);
                await _context.SaveChangesAsync();

                foreach (var sanPhamId in model.SanPhamIds)
                {
                    var sanPham = await _context.SanPhams
                        .AsNoTracking()
                        .FirstOrDefaultAsync(sp => sp.SanPhamId == sanPhamId);

                    if (sanPham == null)
                    {
                        throw new InvalidOperationException($"Không tìm thấy sản phẩm với ID: {sanPhamId}");
                    }

                    var chiTietSuaChua = new ChiTietSuaChua
                    {
                        ChiTietId = await _idGenerator.GenerateIdAsync_Date("CTSC"),
                        SanPhamId = sanPhamId,
                        PhieuSuaChuaId = phieuSuaChua.PhieuSuaChuaId,
                        LoaiDon = sanPham.NgayHetHanBh > DateOnly.FromDateTime(DateTime.Now) ? "BaoHanh" : "SuaChua",
                        SoLuongLinhKien = 0
                    };

                    await _context.ChiTietSuaChuas.AddAsync(chiTietSuaChua);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return (true, "Đăng ký phiếu sửa chữa thành công!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}
