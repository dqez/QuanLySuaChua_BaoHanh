using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;
using QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    public class ThanhPhoesController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;
        private readonly ExcelImportService _importService;

        public ThanhPhoesController(BHSC_DbContext context, IDGenerator generator, ExcelImportService importService)
        {
            _context = context;
            _idGenerator = generator;
            _importService = importService;
        }

        // GET: QuanTriVien/ThanhPhoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ThanhPhos.ToListAsync());
        }

        // GET: QuanTriVien/ThanhPhoes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhPho = await _context.ThanhPhos
                .FirstOrDefaultAsync(m => m.ThanhPhoId == id);
            if (thanhPho == null)
            {
                return NotFound();
            }

            return View(thanhPho);
        }

        // GET: QuanTriVien/ThanhPhoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuanTriVien/ThanhPhoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenThanhPho")] ThanhPho thanhPho)
        {
            thanhPho.ThanhPhoId = await _idGenerator.GenerateThanhPhoIdAsync();
            ModelState.Remove("ThanhPhoId");
            if (ModelState.IsValid)
            {
                _context.Add(thanhPho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thanhPho);
        }

        // GET: QuanTriVien/ThanhPhoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhPho = await _context.ThanhPhos.FindAsync(id);
            if (thanhPho == null)
            {
                return NotFound();
            }
            return View(thanhPho);
        }

        // POST: QuanTriVien/ThanhPhoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ThanhPhoId,TenThanhPho")] ThanhPho thanhPho)
        {
            if (id != thanhPho.ThanhPhoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thanhPho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThanhPhoExists(thanhPho.ThanhPhoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(thanhPho);
        }

        // GET: QuanTriVien/ThanhPhoes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhPho = await _context.ThanhPhos
                .FirstOrDefaultAsync(m => m.ThanhPhoId == id);
            if (thanhPho == null)
            {
                return NotFound();
            }

            return View(thanhPho);
        }

        // POST: QuanTriVien/ThanhPhoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thanhPho = await _context.ThanhPhos.FindAsync(id);
            if (thanhPho != null)
            {
                _context.ThanhPhos.Remove(thanhPho);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThanhPhoExists(string id)
        {
            return _context.ThanhPhos.Any(e => e.ThanhPhoId == id);
        }

        // GET: QuanTriVien/ThanhPhoes/Import
        public IActionResult Import()
        {
            return View(new ImportViewModel { ImportType = "ThanhPho" });
        }

        // POST: QuanTriVien/ThanhPhoes/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ImportViewModel model)
        {
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
                    var result = await _importService.ImportThanhPhoAsync(stream);
                    
                    // Add messages to TempData
                    if (result.SuccessCount > 0)
                    {
                        TempData["SuccessMessage"] = $"Đã nhập thành công {result.SuccessCount} thành phố.";
                    }
                    
                    if (result.SkippedCount > 0)
                    {
                        TempData["WarningMessage"] = $"Đã bỏ qua {result.SkippedCount} thành phố (đã tồn tại).";
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
}
