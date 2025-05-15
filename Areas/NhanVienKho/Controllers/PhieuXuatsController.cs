using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Controllers
{
    [Area("NhanVienKho")]
    public class PhieuXuatsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public PhieuXuatsController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }

        // GET: NhanVienKho/PhieuXuats
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.PhieuXuats.Include(p => p.Kho);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: NhanVienKho/PhieuXuats/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuXuat = await _context.PhieuXuats
                .Include(p => p.Kho)
                .FirstOrDefaultAsync(m => m.PhieuXuatId == id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            return View(phieuXuat);
        }

        // GET: NhanVienKho/PhieuXuats/Create
        public IActionResult Create()
        {
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id");
            return View();
        }

        // POST: NhanVienKho/PhieuXuats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhieuXuatId,KhoId,NgayXuat,TongTien,TrangThai,GhiChu")] PhieuXuat phieuXuat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phieuXuat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuXuat.KhoId);
            return View(phieuXuat);
        }

        // GET: NhanVienKho/PhieuXuats/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuXuat = await _context.PhieuXuats.FindAsync(id);
            if (phieuXuat == null)
            {
                return NotFound();
            }
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuXuat.KhoId);
            return View(phieuXuat);
        }

        // POST: NhanVienKho/PhieuXuats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhieuXuatId,KhoId,NgayXuat,TongTien,TrangThai,GhiChu")] PhieuXuat phieuXuat)
        {
            if (id != phieuXuat.PhieuXuatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuXuat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuXuatExists(phieuXuat.PhieuXuatId))
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
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuXuat.KhoId);
            return View(phieuXuat);
        }

        // GET: NhanVienKho/PhieuXuats/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuXuat = await _context.PhieuXuats
                .Include(p => p.Kho)
                .FirstOrDefaultAsync(m => m.PhieuXuatId == id);
            if (phieuXuat == null)
            {
                return NotFound();
            }

            return View(phieuXuat);
        }

        // POST: NhanVienKho/PhieuXuats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phieuXuat = await _context.PhieuXuats.FindAsync(id);
            if (phieuXuat != null)
            {
                _context.PhieuXuats.Remove(phieuXuat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhieuXuatExists(string id)
        {
            return _context.PhieuXuats.Any(e => e.PhieuXuatId == id);
        }
    }
}
