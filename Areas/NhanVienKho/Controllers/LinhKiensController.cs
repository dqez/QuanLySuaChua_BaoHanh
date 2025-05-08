using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Controllers
{
    [Area("NhanVienKho")]
    public class LinhKiensController : Controller
    {
        private readonly BHSC_DbContext _context;

        public LinhKiensController(BHSC_DbContext context)
        {
            _context = context;
        }

        // GET: NhanVienKho/LinhKiens
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.LinhKiens.Include(l => l.DanhMuc);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: NhanVienKho/LinhKiens/Details/5
        public async Task<IActionResult> Details(int? id)
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
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId");
            return View();
        }

        // POST: NhanVienKho/LinhKiens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LinhKienId,DanhMucId,TenLinhKien,SoLuongTon,DonGia,PhamViSuDung,GhiChu")] LinhKien linhKien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(linhKien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", linhKien.DanhMucId);
            return View(linhKien);
        }

        // GET: NhanVienKho/LinhKiens/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", linhKien.DanhMucId);
            return View(linhKien);
        }

        // POST: NhanVienKho/LinhKiens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LinhKienId,DanhMucId,TenLinhKien,SoLuongTon,DonGia,PhamViSuDung,GhiChu")] LinhKien linhKien)
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
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", linhKien.DanhMucId);
            return View(linhKien);
        }

        // GET: NhanVienKho/LinhKiens/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var linhKien = await _context.LinhKiens.FindAsync(id);
            if (linhKien != null)
            {
                _context.LinhKiens.Remove(linhKien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LinhKienExists(int id)
        {
            return _context.LinhKiens.Any(e => e.LinhKienId == id);
        }
    }
}
