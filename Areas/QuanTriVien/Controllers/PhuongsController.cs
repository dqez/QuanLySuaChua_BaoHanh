using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    public class PhuongsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;
        private readonly ExcelImportService _importService;

        public PhuongsController(BHSC_DbContext context, IDGenerator generator, ExcelImportService importService)
        {
            _context = context;
            _idGenerator = generator;
            _importService = importService;
        }

        // GET: QuanTriVien/Phuongs
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.Phuongs.Include(p => p.Quan).ThenInclude(q => q.ThanhPho);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: QuanTriVien/Phuongs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(m => m.PhuongId == id);
            if (phuong == null)
            {
                return NotFound();
            }

            return View(phuong);
        }

        // GET: QuanTriVien/Phuongs/Create
        public IActionResult Create()
        {
            var PviewModel = new PhuongViewModel
            {
                Phuong = new Phuong(),
                Quans = _context.Quans.Select(q => new SelectListItem
                {
                    Value = q.QuanId,
                    Text = q.QuanId + " - " + q.TenQuan
                }),
                ThanhPhos = _context.ThanhPhos.Select(tp => new SelectListItem
                {
                    Value = tp.ThanhPhoId,
                    Text = tp.ThanhPhoId + " - " + tp.TenThanhPho
                })
            };
            
            return View(PviewModel);
        }

        // POST: QuanTriVien/Phuongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhuongViewModel phuongViewModel)
        {
            phuongViewModel.Phuong.PhuongId = await _idGenerator.GeneratePhuongIdAsync();
            ModelState.Remove("Phuong.PhuongId");
            ModelState.Remove("Phuong.ThanhPhos");
            ModelState.Remove("Phuong.Quans");
            ModelState.Remove("ThanhPhos");
            ModelState.Remove("Quans");

            ModelState.Remove("PhuongId");
            if (ModelState.IsValid)
            {
                _context.Add(phuongViewModel.Phuong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            phuongViewModel.Quans = _context.Quans.Select(q => new SelectListItem
            {
                Value = q.QuanId,
                Text = q.QuanId + " - " + q.TenQuan
            });

            phuongViewModel.ThanhPhos = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId,
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho
            });
            return View(phuongViewModel);
        }

        // GET: QuanTriVien/Phuongs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(m => m.PhuongId == id);
                
            if (phuong == null)
            {
                return NotFound();
            }
            
            // Create view model for edit
            var viewModel = new PhuongViewModel
            {
                Phuong = phuong,
                ThanhPhos = _context.ThanhPhos.Select(tp => new SelectListItem
                {
                    Value = tp.ThanhPhoId,
                    Text = tp.ThanhPhoId + " - " + tp.TenThanhPho,
                    Selected = tp.ThanhPhoId == phuong.Quan.ThanhPhoId
                }),
                Quans = _context.Quans.Where(q => q.ThanhPhoId == phuong.Quan.ThanhPhoId).Select(q => new SelectListItem
                {
                    Value = q.QuanId,
                    Text = q.QuanId + " - " + q.TenQuan,
                    Selected = q.QuanId == phuong.QuanId
                })
            };
            
            return View(viewModel);
        }

        // POST: QuanTriVien/Phuongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PhuongViewModel viewModel)
        {
            if (id != viewModel.Phuong.PhuongId)
            {
                return NotFound();
            }

            ModelState.Remove("Phuong.Quan");
            ModelState.Remove("ThanhPhos");
            ModelState.Remove("Quans");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Phuong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhuongExists(viewModel.Phuong.PhuongId))
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
            
            // Reload dropdown items
            viewModel.ThanhPhos = _context.ThanhPhos.Select(tp => new SelectListItem
            {
                Value = tp.ThanhPhoId,
                Text = tp.ThanhPhoId + " - " + tp.TenThanhPho
            });

            var thanhPhoId = viewModel.Phuong.Quan?.ThanhPhoId;

            viewModel.Quans = _context.Quans
                .Where(q => q.ThanhPhoId == thanhPhoId)
                .Select(q => new SelectListItem
                {
                    Value = q.QuanId,
                    Text = q.QuanId + " - " + q.TenQuan
                });


            return View(viewModel);
        }

        // GET: QuanTriVien/Phuongs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(m => m.PhuongId == id);
            if (phuong == null)
            {
                return NotFound();
            }

            return View(phuong);
        }

        // POST: QuanTriVien/Phuongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phuong = await _context.Phuongs.FindAsync(id);
            if (phuong != null)
            {
                _context.Phuongs.Remove(phuong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhuongExists(string id)
        {
            return _context.Phuongs.Any(e => e.PhuongId == id);
        }

        [HttpGet]
        public JsonResult GetQuansByThanhPho(string thanhPhoId)
        {
            var quans = _context.Quans
                .Where(q => q.ThanhPhoId == thanhPhoId)
                .Select(q => new { q.QuanId, q.TenQuan })
                .ToList();

            return Json(quans);
        }

        // GET: QuanTriVien/Phuongs/Import
        public IActionResult Import()
        {
            return View(new ImportViewModel { ImportType = "Phuong" });
        }

        // POST: QuanTriVien/Phuongs/Import
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
                    var result = await _importService.ImportPhuongAsync(stream);
                    
                    // Add messages to TempData
                    if (result.SuccessCount > 0)
                    {
                        TempData["SuccessMessage"] = $"Đã nhập thành công {result.SuccessCount} phường.";
                    }
                    
                    if (result.SkippedCount > 0)
                    {
                        TempData["WarningMessage"] = $"Đã bỏ qua {result.SkippedCount} phường (đã tồn tại).";
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
