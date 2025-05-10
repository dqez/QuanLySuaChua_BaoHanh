using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{     
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class HomeController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly BHSC_DbContext _context;

        public HomeController(
            UserManager<NguoiDung> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            BHSC_DbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Thống kê số lượng
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalRoles = await _roleManager.Roles.CountAsync();
            ViewBag.TotalProducts = await _context.SanPhams.CountAsync();
            ViewBag.TotalOrders = await _context.PhieuSuaChuas.CountAsync();

            // Danh sách đơn hàng mới nhất
            var latestOrders = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .OrderByDescending(p => p.NgayGui)
                .Take(5)
                .ToListAsync();

            return View(latestOrders);
        }
    }
}
