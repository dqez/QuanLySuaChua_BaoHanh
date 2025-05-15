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
    public class PhieuNhapsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public PhieuNhapsController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }

        // GET: NhanVienKho/PhieuNhaps
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.PhieuNhaps.Include(p => p.Kho);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: NhanVienKho/PhieuNhaps/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.Kho)
                .FirstOrDefaultAsync(m => m.PhieuNhapId == id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            return View(phieuNhap);
        }

        // GET: NhanVienKho/PhieuNhaps/Create
        public IActionResult Create()
        {
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id");
            return View();
        }

        // POST: NhanVienKho/PhieuNhaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhieuNhapId,KhoId,NgayNhap,TongTien,TrangThai,GhiChu")] PhieuNhap phieuNhap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phieuNhap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuNhap.KhoId);
            return View(phieuNhap);
        }

        // GET: NhanVienKho/PhieuNhaps/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuNhap = await _context.PhieuNhaps.FindAsync(id);
            if (phieuNhap == null)
            {
                return NotFound();
            }
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuNhap.KhoId);
            return View(phieuNhap);
        }

        // POST: NhanVienKho/PhieuNhaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhieuNhapId,KhoId,NgayNhap,TongTien,TrangThai,GhiChu")] PhieuNhap phieuNhap)
        {
            if (id != phieuNhap.PhieuNhapId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuNhap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuNhapExists(phieuNhap.PhieuNhapId))
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
            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuNhap.KhoId);
            return View(phieuNhap);
        }

        // GET: NhanVienKho/PhieuNhaps/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.Kho)
                .FirstOrDefaultAsync(m => m.PhieuNhapId == id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            return View(phieuNhap);
        }

        // POST: NhanVienKho/PhieuNhaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phieuNhap = await _context.PhieuNhaps.FindAsync(id);
            if (phieuNhap != null)
            {
                _context.PhieuNhaps.Remove(phieuNhap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhieuNhapExists(string id)
        {
            return _context.PhieuNhaps.Any(e => e.PhieuNhapId == id);
        }
    }
}
