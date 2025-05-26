using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize(Roles = "KhachHang")]
    public class DonSuaChuaController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly BHSC_DbContext _context;

        public DonSuaChuaController(
            UserManager<NguoiDung> userManager,
            BHSC_DbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var allOrders = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.SanPham)
                .Where(p => p.KhachHangId == user.Id)
                .OrderByDescending(p => p.NgayGui)
                .ToListAsync();            ViewBag.AllOrders = allOrders;
            ViewBag.PendingOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.ChoXacNhan.ToString()).ToList();
            ViewBag.ConfirmedOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.DaXacNhan.ToString() || o.TrangThai == Enums.TrangThaiPhieu.DaPhanCong.ToString()).ToList();
            ViewBag.WaitingForInspectionOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.ChoKiemTra.ToString()).ToList();
            ViewBag.RepairingOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.DangSuaChua.ToString()).ToList();
            ViewBag.CompletedOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.HoanThanh.ToString() || o.TrangThai == Enums.TrangThaiPhieu.DaSuaXong.ToString() || o.TrangThai == Enums.TrangThaiPhieu.DaThanhToan.ToString()).ToList();
            ViewBag.CancelledOrders = allOrders.Where(o => o.TrangThai == Enums.TrangThaiPhieu.DaHuy.ToString()).ToList();

         
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập lại" });
            }

            var order = await _context.PhieuSuaChuas
                .FirstOrDefaultAsync(p => p.PhieuSuaChuaId == id && p.KhachHangId == user.Id);

            if (order == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn sửa chữa" });
            }

            if (order.TrangThai != Enums.TrangThaiPhieu.ChoXacNhan.ToString())
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn sửa chữa đang chờ xác nhận" });
            }
            
            // Update trạng thái phiếu sửa chữa thành Đã Hủy
            order.TrangThai = Enums.TrangThaiPhieu.DaHuy.ToString();
            
            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Hủy đơn sửa chữa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi hủy đơn sửa chữa: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ChiTiet(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            var order = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.KyThuat)
                .Include(p => p.Phuong)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.SanPham)
                        .ThenInclude(s => s.DanhMuc) 
                .FirstOrDefaultAsync(p => p.PhieuSuaChuaId == id && p.KhachHangId == user.Id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
