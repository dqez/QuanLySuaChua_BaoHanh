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
    public class QuansController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;
        private readonly ExcelImportService _importService;

        public QuansController(BHSC_DbContext context, IDGenerator generator, ExcelImportService importService)
        {
            _context = context;
            _idGenerator = generator;
            _importService = importService;
        }

        // GET: QuanTriVien/Quans
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.Quans.Include(q => q.ThanhPho);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: QuanTriVien/Quans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quan = await _context.Quans
                .Include(q => q.ThanhPho)
                .FirstOrDefaultAsync(m => m.QuanId == id);
            if (quan == null)
            {
                return NotFound();
            }

            return View(quan);
        }

        // GET: QuanTriVien/Quans/Create
        public IActionResult Create()
        {
            var thanhPhoList = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId.ToString(),
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho
            }).ToList();

            ViewData["ThanhPhoId"] = thanhPhoList;
            return View();
        }

        // POST: QuanTriVien/Quans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenQuan,ThanhPhoId")] Quan quan)
        {
            quan.QuanId = await _idGenerator.GenerateQuanIdAsync();
            ModelState.Remove("QuanId");
            if (ModelState.IsValid)
            {
                _context.Add(quan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var thanhPhoList = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId.ToString(),
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho,
                Selected = (tp.ThanhPhoId == quan.ThanhPhoId)
            }).ToList();

            ViewData["ThanhPhoId"] = thanhPhoList;
            return View(quan);
        }

        // GET: QuanTriVien/Quans/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quan = await _context.Quans.FindAsync(id);
            if (quan == null)
            {
                return NotFound();
            }
            var thanhPhoList = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId.ToString(),
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho,
                Selected = (tp.ThanhPhoId == quan.ThanhPhoId)
            }).ToList();

            ViewData["ThanhPhoId"] = thanhPhoList;
            return View(quan);
        }

        // POST: QuanTriVien/Quans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("QuanId,TenQuan,ThanhPhoId")] Quan quan)
        {
            if (id != quan.QuanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanExists(quan.QuanId))
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
            var thanhPhoList = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId.ToString(),
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho,
                Selected = (tp.ThanhPhoId == quan.ThanhPhoId)
            }).ToList();

            ViewData["ThanhPhoId"] = thanhPhoList;
            return View(quan);
        }

        // GET: QuanTriVien/Quans/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quan = await _context.Quans
                .Include(q => q.ThanhPho)
                .FirstOrDefaultAsync(m => m.QuanId == id);
            if (quan == null)
            {
                return NotFound();
            }

            return View(quan);
        }

        // POST: QuanTriVien/Quans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var quan = await _context.Quans.FindAsync(id);
            if (quan != null)
            {
                _context.Quans.Remove(quan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuanExists(string id)
        {
            return _context.Quans.Any(e => e.QuanId == id);
        }

        // Method to check if Quan exists by name and ThanhPho
        public async Task<Quan> FindQuanByNameAsync(string tenQuan, string thanhPhoId)
        {
            return await _context.Quans
                .FirstOrDefaultAsync(q => q.TenQuan.ToLower() == tenQuan.ToLower() && 
                                         q.ThanhPhoId == thanhPhoId);
        }

        // Method to create new Quan
        public async Task<Quan> CreateQuanAsync(string tenQuan, string thanhPhoId)
        {
            var quan = new Quan
            {
                QuanId = await _idGenerator.GenerateQuanIdAsync(),
                TenQuan = tenQuan,
                ThanhPhoId = thanhPhoId
            };
            _context.Quans.Add(quan);
            await _context.SaveChangesAsync();
            return quan;
        }

        // GET: QuanTriVien/Quans/Import
        public IActionResult Import()
        {
            return View(new ImportViewModel { ImportType = "Quan" });
        }

        // POST: QuanTriVien/Quans/Import
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
                    var result = await _importService.ImportQuanAsync(stream);
                    
                    // Add messages to TempData
                    if (result.SuccessCount > 0)
                    {
                        TempData["SuccessMessage"] = $"Đã nhập thành công {result.SuccessCount} quận.";
                    }
                    
                    if (result.SkippedCount > 0)
                    {
                        TempData["WarningMessage"] = $"Đã bỏ qua {result.SkippedCount} quận (đã tồn tại).";
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
}
