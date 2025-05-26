using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.KyThuatVien.Controllers
{
    [Area("KyThuatVien")]
    [Authorize(Roles = "KyThuatVien")]
    public class HomeController : Controller
    {
        private readonly BHSC_DbContext _context;

        public HomeController(BHSC_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy user ID của kỹ thuật viên đăng nhập (cần triển khai logic xác thực)
            string kyThuatVienId = "KTV001";     
            // Số đơn được phân công chưa bắt đầu xử lý
            int assignedCount = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == kyThuatVienId && 
                      (p.TrangThai == TrangThaiPhieu.DaPhanCong.ToString()))
                .CountAsync();

            // Số đơn đang xử lý
            int inProgressCount = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == kyThuatVienId && 
                      (p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString()))
                .CountAsync();

            // Số đơn đã sửa xong chờ thanh toán hoặc đã thanh toán
            int completedTodayCount = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == kyThuatVienId && 
                      (p.TrangThai == TrangThaiPhieu.DaSuaXong.ToString() || 
                       p.TrangThai == TrangThaiPhieu.DaThanhToan.ToString()) &&
                       p.NgayTra.HasValue && 
                       p.NgayTra.Value.Date == DateTime.Today)
                .CountAsync();

            // Linh kiện sắp hết hàng
            int lowStockCount = await _context.LinhKiens
                .Where(l => l.SoLuongTon < 5)
                .CountAsync();

            // Đơn gần đây
            var recentOrders = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == kyThuatVienId)
                .Include(p => p.KhachHang)
                .OrderByDescending(p => p.NgayGui)
                .Take(5)
                .ToListAsync();

            // Linh kiện sử dụng nhiều
            var topComponents = await _context.ChiTietSuaChuas
                .Where(c => c.LinhKienId != null)
                .GroupBy(c => c.LinhKienId)
                .Select(g => new
                {
                    LinhKienId = g.Key,
                    Count = g.Sum(c => c.SoLuongLinhKien)
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var topComponentsDetails = new List<dynamic>();
            foreach (var item in topComponents)
            {
                var linhKien = await _context.LinhKiens
                    .FirstOrDefaultAsync(l => l.LinhKienId == item.LinhKienId);
                
                if (linhKien != null)
                {
                    topComponentsDetails.Add(new
                    {
                        linhKien.LinhKienId,
                        linhKien.TenLinhKien,
                        linhKien.SoLuongTon,
                        UsageCount = item.Count
                    });
                }
            }

            ViewBag.AssignedCount = assignedCount;
            ViewBag.InProgressCount = inProgressCount;
            ViewBag.CompletedTodayCount = completedTodayCount;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.TopComponents = topComponentsDetails;

            return View();
        }
    }
}
