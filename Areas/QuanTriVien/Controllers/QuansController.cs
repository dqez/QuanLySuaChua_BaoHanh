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
    public class QuansController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public QuansController(BHSC_DbContext context, IDGenerator generator)
        {
            _context = context;
            _idGenerator = generator;
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
    }
}
