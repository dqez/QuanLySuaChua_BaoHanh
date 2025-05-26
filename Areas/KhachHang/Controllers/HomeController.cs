using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize(Roles = "KhachHang")]
    public class HomeController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly BHSC_DbContext _context;

        public HomeController(
            UserManager<NguoiDung> userManager,
            BHSC_DbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy danh sách phiếu sửa chữa của khách hàng
            var orders = await _context.PhieuSuaChuas
                .Where(p => p.KhachHangId == user.Id)
                .OrderByDescending(p => p.NgayGui)
                .ToListAsync();

            // Lấy danh sách sản phẩm của khách hàng
            var products = await _context.SanPhams
                .Where(p => p.KhachHangId == user.Id)
                .ToListAsync();

            // Lấy thông tin địa chỉ đầy đủ
            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(p => p.PhuongId == user.PhuongId);

            ViewBag.DiaChi = phuong != null
                ? $"{user.DiaChi}, {phuong.TenPhuong}, {phuong.Quan.TenQuan}, {phuong.Quan.ThanhPho.TenThanhPho}"
                : user.DiaChi;
            ViewBag.Orders = orders;
            ViewBag.Products = products;

            return View(user);
        }
    }
}
