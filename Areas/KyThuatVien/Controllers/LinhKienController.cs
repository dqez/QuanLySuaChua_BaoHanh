using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.KyThuatVien.Controllers
{
    [Area("KyThuatVien")]
    [Authorize(Roles = "KyThuatVien")]
    public class LinhKienController : Controller
    {
        private readonly BHSC_DbContext _context;

        public LinhKienController(BHSC_DbContext context)
        {
            _context = context;
        }

        // GET: KyThuatVien/LinhKien
        public async Task<IActionResult> Index()
        {
            var linhKiens = await _context.LinhKiens
                .Include(l => l.DanhMuc)
                .OrderBy(l => l.DanhMucId)
                .ThenBy(l => l.TenLinhKien)
                .ToListAsync();

            return View(linhKiens);
        }

        // GET: KyThuatVien/LinhKien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linhKien = await _context.LinhKiens
                .Include(l => l.DanhMuc)
                .Include(l => l.ChiTietSuaChuas)
                    .ThenInclude(c => c.PhieuSuaChua)
                .FirstOrDefaultAsync(m => m.LinhKienId == id);

            if (linhKien == null)
            {
                return NotFound();
            }

            return View(linhKien);
        }

        // GET: KyThuatVien/LinhKien/ThongKeSuDung
        public async Task<IActionResult> ThongKeSuDung()
        {
            var linhKienStats = await _context.ChiTietSuaChuas
                .Where(c => c.LinhKienId != null)
                .GroupBy(c => c.LinhKienId)
                .Select(g => new
                {
                    LinhKienId = g.Key,
                    TotalUsed = g.Sum(c => c.SoLuongLinhKien)
                })
                .OrderByDescending(x => x.TotalUsed)
                .Take(20)
                .ToListAsync();

            var result = new List<object>();
            foreach (var stat in linhKienStats)
            {
                var linhKien = await _context.LinhKiens
                    .Include(l => l.DanhMuc)
                    .FirstOrDefaultAsync(l => l.LinhKienId == stat.LinhKienId);

                if (linhKien != null)
                {
                    result.Add(new
                    {
                        linhKien.LinhKienId,
                        linhKien.TenLinhKien,
                        DanhMuc = linhKien.DanhMuc?.TenDanhMuc ?? "Không có danh mục",
                        linhKien.SoLuongTon,
                        TotalUsed = stat.TotalUsed
                    });
                }
            }

            return View(result);
        }
    }
}
