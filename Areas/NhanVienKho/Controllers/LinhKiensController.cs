using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Models;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Controllers
{
    [Area("NhanVienKho")]
    [Authorize(Roles = "NhanVienKho")]
    public class LinhKiensController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public LinhKiensController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }
        // GET: NhanVienKho/LinhKiens
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;
            var linhKienQuery = _context.LinhKiens.AsQueryable();

            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                linhKienQuery = linhKienQuery.Where(lk =>
                    lk.LinhKienId.Contains(searchString) ||
                    lk.TenLinhKien.Contains(searchString));
            }

            linhKienQuery = linhKienQuery.OrderBy(lk => lk.TenLinhKien);

            return View(await PaginatedList<LinhKien>.CreateAsync(
                linhKienQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: NhanVienKho/LinhKiens/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linhKien = await _context.LinhKiens
                .Include(l => l.DanhMuc)
                .FirstOrDefaultAsync(m => m.LinhKienId == id);
            if (linhKien == null)
            {
                return NotFound();
            }

            return View(linhKien);
        }

        // GET: NhanVienKho/LinhKiens/Create
        public IActionResult Create()
        {
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "TenDanhMuc");
            return View();
        }

        // POST: NhanVienKho/LinhKiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DanhMucId,TenLinhKien,SoLuongTon,DonGia,PhamViSuDung,GhiChu")] LinhKien linhKien)
        {
            linhKien.LinhKienId = await _idGenerator.GenerateLinhKienIdAsync();
            ModelState.Remove("LinhKienId");
            if (ModelState.IsValid)
            {
                _context.Add(linhKien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "TenDanhMuc", linhKien.DanhMucId);

            return View(linhKien);
        }

        // GET: NhanVienKho/LinhKiens/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linhKien = await _context.LinhKiens.FindAsync(id);
            if (linhKien == null)
            {
                return NotFound();
            }
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "TenDanhMuc", linhKien.DanhMucId);
            return View(linhKien);
        }

        // POST: NhanVienKho/LinhKiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LinhKienId,DanhMucId,TenLinhKien,SoLuongTon,DonGia,PhamViSuDung,GhiChu")] LinhKien linhKien)
        {
            if (id != linhKien.LinhKienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(linhKien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinhKienExists(linhKien.LinhKienId))
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
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "TenDanhMuc", linhKien.DanhMucId);
            return View(linhKien);
        }

        // GET: NhanVienKho/LinhKiens/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linhKien = await _context.LinhKiens
                .Include(l => l.DanhMuc)
                .FirstOrDefaultAsync(m => m.LinhKienId == id);
            if (linhKien == null)
            {
                return NotFound();
            }

            return View(linhKien);
        }

        // POST: NhanVienKho/LinhKiens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var linhKien = await _context.LinhKiens.FindAsync(id);
            if (linhKien != null)
            {
                _context.LinhKiens.Remove(linhKien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LinhKienExists(string id)
        {
            return _context.LinhKiens.Any(e => e.LinhKienId == id);
        }
    }
}
