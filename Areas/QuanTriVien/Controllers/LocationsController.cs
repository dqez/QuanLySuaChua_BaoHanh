using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models;
using QuanLySuaChua_BaoHanh.Services;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class LocationsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly LocationsImportService _locationsImportService;

        public LocationsController(
            BHSC_DbContext context,
            LocationsImportService locationsImportService)
        {
            _context = context;
            _locationsImportService = locationsImportService;
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

        // GET: QuanTriVien/Locations/ImportAll
        public IActionResult ImportAll()
        {
            return View(new ImportViewModel { ImportType = "AllLocations" });
        }

        // POST: QuanTriVien/Locations/ImportAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportAll(ImportViewModel model)
        {
            // Ensure ImportType is set
            if (string.IsNullOrEmpty(model.ImportType))
            {
                model.ImportType = "AllLocations";
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Vui lòng chọn file");
                return View(model);
            }

            // Check file extension
            var fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
            if (fileExtension != ".xlsx" && fileExtension != ".xls" && fileExtension != ".csv")
            {
                ModelState.AddModelError("File", "Chỉ chấp nhận file Excel (.xlsx, .xls) hoặc CSV (.csv)");
                return View(model);
            }

            try
            {
                using (var stream = model.File.OpenReadStream())
                {
                    var result = await _locationsImportService.ImportAllLocationsAsync(stream, model.File.FileName);
                    
                    // Add messages to TempData
                    if (result.SuccessCount > 0)
                    {
                        TempData["SuccessMessage"] = $"Đã nhập thành công {result.SuccessCount} vị trí (thành phố, quận, phường).";
                    }
                    
                    if (result.SkippedCount > 0)
                    {
                        TempData["WarningMessage"] = $"Đã bỏ qua {result.SkippedCount} vị trí (đã tồn tại).";
                    }
                    
                    if (result.Warnings.Any())
                    {
                        TempData["WarningDetails"] = string.Join("<br/>", result.Warnings);
                    }
                    
                    if (result.Errors.Any())
                    {
                        TempData["ErrorMessage"] = string.Join("<br/>", result.Errors);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi nhập dữ liệu: {ex.Message}");
                return View(model);
            }
        }
    }

    public class LocationsOverviewViewModel
    {
        public IEnumerable<ThanhPho> ThanhPhoes { get; set; }
        public IEnumerable<Quan> Quans { get; set; }
        public IEnumerable<Phuong> Phuongs { get; set; }
    }
}
