using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    public class PhuongsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public PhuongsController(BHSC_DbContext context, IDGenerator generator)
        {
            _context = context;
            _idGenerator = generator;
        }

        // GET: QuanTriVien/Phuongs
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.Phuongs.Include(p => p.Quan);
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
            ViewData["QuanId"] = new SelectList(_context.Quans, "QuanId", "QuanId");
            return View();
        }

        // POST: QuanTriVien/Phuongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenPhuong,QuanId")] Phuong phuong)
        {
            phuong.PhuongId = await _idGenerator.GeneratePhuongIdAsync();

            ModelState.Remove("PhuongId");
            if (ModelState.IsValid)
            {
                _context.Add(phuong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuanId"] = new SelectList(_context.Quans, "QuanId", "QuanId", phuong.QuanId);
            return View(phuong);
        }

        // GET: QuanTriVien/Phuongs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phuong = await _context.Phuongs.FindAsync(id);
            if (phuong == null)
            {
                return NotFound();
            }
            ViewData["QuanId"] = new SelectList(_context.Quans, "QuanId", "QuanId", phuong.QuanId);
            return View(phuong);
        }

        // POST: QuanTriVien/Phuongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhuongId,TenPhuong,QuanId")] Phuong phuong)
        {
            if (id != phuong.PhuongId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phuong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhuongExists(phuong.PhuongId))
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
            ViewData["QuanId"] = new SelectList(_context.Quans, "QuanId", "QuanId", phuong.QuanId);
            return View(phuong);
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
    }
}
