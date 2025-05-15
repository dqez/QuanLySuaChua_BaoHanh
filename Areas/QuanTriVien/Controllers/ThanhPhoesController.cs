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
    public class ThanhPhoesController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public ThanhPhoesController(BHSC_DbContext context, IDGenerator generator)
        {
            _context = context;
            _idGenerator = generator;
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
    }
}
