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
    public class ChiTietPxesController : Controller
    {
        private readonly BHSC_DbContext _context;

        public ChiTietPxesController(BHSC_DbContext context)
        {
            _context = context;
        }

        // GET: NhanVienKho/ChiTietPxes
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.ChiTietPxes.Include(c => c.PhieuSuaChua).Include(c => c.PhieuXuat);
            return View(await bHSC_DbContext.ToListAsync());
        }

        //// GET: NhanVienKho/ChiTietPxes/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var chiTietPx = await _context.ChiTietPxes
        //        .Include(c => c.PhieuSuaChua)
        //        .Include(c => c.PhieuXuat)
        //        .FirstOrDefaultAsync(m => m.PhieuXuatId == id);
        //    if (chiTietPx == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(chiTietPx);
        //}

        // GET: NhanVienKho/ChiTietPxes/Create
        public IActionResult Create()
        {
            ViewData["PhieuSuaChuaId"] = new SelectList(_context.PhieuSuaChuas, "PhieuSuaChuaId", "PhieuSuaChuaId");
            ViewData["PhieuXuatId"] = new SelectList(_context.PhieuXuats, "PhieuXuatId", "PhieuXuatId");
            return View();
        }

        // POST: NhanVienKho/ChiTietPxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhieuXuatId,PhieuSuaChuaId,GhiChu")] ChiTietPx chiTietPx)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietPx);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhieuSuaChuaId"] = new SelectList(_context.PhieuSuaChuas, "PhieuSuaChuaId", "PhieuSuaChuaId", chiTietPx.PhieuSuaChuaId);
            ViewData["PhieuXuatId"] = new SelectList(_context.PhieuXuats, "PhieuXuatId", "PhieuXuatId", chiTietPx.PhieuXuatId);
            return View(chiTietPx);
        }

        // GET: NhanVienKho/ChiTietPxes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPx = await _context.ChiTietPxes.FindAsync(id);
            if (chiTietPx == null)
            {
                return NotFound();
            }
            ViewData["PhieuSuaChuaId"] = new SelectList(_context.PhieuSuaChuas, "PhieuSuaChuaId", "PhieuSuaChuaId", chiTietPx.PhieuSuaChuaId);
            ViewData["PhieuXuatId"] = new SelectList(_context.PhieuXuats, "PhieuXuatId", "PhieuXuatId", chiTietPx.PhieuXuatId);
            return View(chiTietPx);
        }

        // POST: NhanVienKho/ChiTietPxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhieuXuatId,PhieuSuaChuaId,GhiChu")] ChiTietPx chiTietPx)
        {
            if (id != chiTietPx.PhieuXuatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietPx);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietPxExists(chiTietPx.PhieuXuatId))
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
            ViewData["PhieuSuaChuaId"] = new SelectList(_context.PhieuSuaChuas, "PhieuSuaChuaId", "PhieuSuaChuaId", chiTietPx.PhieuSuaChuaId);
            ViewData["PhieuXuatId"] = new SelectList(_context.PhieuXuats, "PhieuXuatId", "PhieuXuatId", chiTietPx.PhieuXuatId);
            return View(chiTietPx);
        }

        // GET: NhanVienKho/ChiTietPxes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPx = await _context.ChiTietPxes
                .Include(c => c.PhieuSuaChua)
                .Include(c => c.PhieuXuat)
                .FirstOrDefaultAsync(m => m.PhieuXuatId == id);
            if (chiTietPx == null)
            {
                return NotFound();
            }

            return View(chiTietPx);
        }

        // POST: NhanVienKho/ChiTietPxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chiTietPx = await _context.ChiTietPxes.FindAsync(id);
            if (chiTietPx != null)
            {
                _context.ChiTietPxes.Remove(chiTietPx);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietPxExists(string id)
        {
            return _context.ChiTietPxes.Any(e => e.PhieuXuatId == id);
        }
    }
}
