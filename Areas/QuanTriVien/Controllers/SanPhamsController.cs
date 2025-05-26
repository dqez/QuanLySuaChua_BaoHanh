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
    public class SanPhamsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public SanPhamsController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }

        // GET: QuanTriVien/SanPhams
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.SanPhams.Include(s => s.DanhMuc).Include(s => s.KhachHang);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: QuanTriVien/SanPhams/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.KhachHang)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // GET: QuanTriVien/SanPhams/Create
        public IActionResult Create()
        {
            ViewBag.KhachHangId = _context.NguoiDungs
                .Where(nd => nd.VaiTro == "KhachHang")
                .Select(nd => new SelectListItem
                {
                    Value = nd.Id.ToString(),
                    Text = nd.Id + " - " + nd.HoTen
                })
                .ToList();

            ViewBag.DanhMucId = _context.DanhMucs
                .Select(dm => new SelectListItem
                {
                    Value = dm.DanhMucId,
                    Text = dm.DanhMucId + " - " + dm.TenDanhMuc
                }
                    )
                .ToList();

            //ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId");
            //ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id");
            return View();
        }

        // POST: QuanTriVien/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SanPhamId,KhachHangId,DanhMucId,MaBh,TenSanPham,NgayMua,ThoiGianBaoHanh,NgayHetHanBh,MoTa")] SanPham sanPham)
        {
            if (sanPham.SanPhamId == null)
            {
                sanPham.SanPhamId = await _idGenerator.GenerateSanPhamIdAsync();
            }

            ModelState.Remove(nameof(sanPham.SanPhamId));

            if (ModelState.IsValid)
            {
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", sanPham.DanhMucId);
            ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id", sanPham.KhachHangId);
            return View(sanPham);
        }

        // GET: QuanTriVien/SanPhams/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewBag.KhachHangId = _context.NguoiDungs
                .Where(nd => nd.VaiTro == "KhachHang")
                .Select(nd => new SelectListItem
                {
                    Value = nd.Id.ToString(),
                    Text = nd.Id + " - " + nd.HoTen,
                    //Selected = sanPham.KhachHangId == nd.Id //aspnetcore sẽ tự động.
                })
                .ToList();

            ViewBag.DanhMucId = _context.DanhMucs
                .Select(dm => new SelectListItem
                {
                    Value = dm.DanhMucId,
                    Text = dm.DanhMucId + " - " + dm.TenDanhMuc,
                    //Selected = sanPham.DanhMucId == dm.DanhMucId
                }
                )
                .ToList();
            //ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", sanPham.DanhMucId);
            //ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id", sanPham.KhachHangId);
            return View(sanPham);
        }

        // POST: QuanTriVien/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SanPhamId,KhachHangId,DanhMucId,MaBh,TenSanPham,NgayMua,ThoiGianBaoHanh,NgayHetHanBh,MoTa")] SanPham sanPham)
        {
            if (id != sanPham.SanPhamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.SanPhamId))
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
            ViewData["DanhMucId"] = new SelectList(_context.DanhMucs, "DanhMucId", "DanhMucId", sanPham.DanhMucId);
            ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id", sanPham.KhachHangId);
            return View(sanPham);
        }

        // GET: QuanTriVien/SanPhams/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.KhachHang)
                .FirstOrDefaultAsync(m => m.SanPhamId == id);
            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

        // POST: QuanTriVien/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(string id)
        {
            return _context.SanPhams.Any(e => e.SanPhamId == id);
        }
    }
}
