using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class LocationsController : Controller
    {
        private readonly BHSC_DbContext _context;

        public LocationsController(BHSC_DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách 5 thành phố
            var thanhPhoes = await _context.ThanhPhos
                .OrderBy(tp => tp.TenThanhPho)
                .Take(5)
                .ToListAsync();
                
            // Lấy danh sách 5 quận, kèm theo thông tin thành phố
            var quans = await _context.Quans
                .Include(q => q.ThanhPho)
                .OrderBy(q => q.TenQuan)
                .Take(5)
                .ToListAsync();

            // Lấy danh sách 5 phường, kèm theo thông tin quận và thành phố
            var phuongs = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .OrderBy(p => p.TenPhuong)
                .Take(5)
                .ToListAsync();

            var model = new LocationsOverviewViewModel
            {
                ThanhPhoes = thanhPhoes,
                Quans = quans,
                Phuongs = phuongs
            };

            return View(model);
        }
    }

    public class LocationsOverviewViewModel
    {
        public IEnumerable<ThanhPho> ThanhPhoes { get; set; }
        public IEnumerable<Quan> Quans { get; set; }
        public IEnumerable<Phuong> Phuongs { get; set; }
    }
}
