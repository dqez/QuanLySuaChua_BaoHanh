using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
namespace QuanLySuaChua_BaoHanh.Areas.KyThuatVien.Controllers
{
    [Area("KyThuatVien")]
    [Authorize(Roles = "KyThuatVien")]
    public class CaNhanController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public CaNhanController(BHSC_DbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Thống kê công việc
            var stats = new Dictionary<string, object>();
            
            // Tổng số đơn đã xử lý
            stats["TotalOrders"] = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id)
                .CountAsync();
            
            // Số đơn đã hoàn thành
            stats["CompletedOrders"] = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id && p.TrangThai == "DaSuaXong")
                .CountAsync();
            
            // Số đơn đang xử lý
            stats["InProgressOrders"] = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id && p.TrangThai == "DangSuaChua")
                .CountAsync();
            
            // Số linh kiện đã sử dụng
            stats["TotalComponents"] = await _context.ChiTietSuaChuas
                .Where(c => c.PhieuSuaChua.KyThuatId == user.Id && c.LinhKienId != null)
                .SumAsync(c => c.SoLuongLinhKien);

            ViewBag.Stats = stats;
            
            return View(user);
        }

        // GET: KyThuatVien/CaNhan/CapNhatThongTin
        public async Task<IActionResult> CapNhatThongTin()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            return View(user);
        }

        // POST: KyThuatVien/CaNhan/CapNhatThongTin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThongTin(NguoiDung model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Cập nhật thông tin cá nhân (không cập nhật các thông tin nhạy cảm như mật khẩu)
            user.HoTen = model.HoTen;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.DiaChi = model.DiaChi;

            _context.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
