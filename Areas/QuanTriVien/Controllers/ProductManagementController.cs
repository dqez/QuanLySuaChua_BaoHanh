using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class ProductManagementController : Controller
    {
        private readonly BHSC_DbContext _context;

        public ProductManagementController(BHSC_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách 5 danh mục
            var danhMucs = await _context.DanhMucs
                .OrderBy(dm => dm.TenDanhMuc)
                .Take(5)
                .ToListAsync();
                
            // Lấy danh sách 5 sản phẩm, kèm theo thông tin danh mục
            var sanPhams = await _context.SanPhams
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.KhachHang)
                .OrderByDescending(sp => sp.NgayMua)
                .Take(5)
                .ToListAsync();

            var model = new ProductsOverviewViewModel
            {
                DanhMucs = danhMucs,
                SanPhams = sanPhams
            };

            return View(model);
        }
    }

    public class ProductsOverviewViewModel
    {
        public IEnumerable<DanhMuc> DanhMucs { get; set; }
        public IEnumerable<SanPham> SanPhams { get; set; }
    }
}
