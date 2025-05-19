using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    public class AssignTasksController : Controller
    {
        private readonly BHSC_DbContext _context;

        public AssignTasksController(BHSC_DbContext context)
        {
            _context = context;
        }

        // GET: QuanTriVien/AssignTasks
        public async Task<IActionResult> Index()
        {
            var bHSC_DbContext = _context.PhieuSuaChuas.Include(p => p.KhachHang).Include(p => p.KyThuat).Include(p => p.Phuong);
            return View(await bHSC_DbContext.ToListAsync());
        }

        // GET: QuanTriVien/AssignTasks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuSuaChua = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.KyThuat)
                .Include(p => p.Phuong)
                .FirstOrDefaultAsync(m => m.PhieuSuaChuaId == id);
            if (phieuSuaChua == null)
            {
                return NotFound();
            }

            return View(phieuSuaChua);
        }


        //QuanTriVien KHONG TAO PHIEUSUACHUA

        //// GET: QuanTriVien/AssignTasks/Create
        //public IActionResult Create()
        //{
        //    ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id");
        //    ViewData["KyThuatId"] = new SelectList(_context.NguoiDungs, "Id", "Id");
        //    ViewData["PhuongId"] = new SelectList(_context.Phuongs, "PhuongId", "PhuongId");
        //    return View();
        //}

        // POST: QuanTriVien/AssignTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PhieuSuaChuaId,KhachHangId,KyThuatId,PhuongId,MoTaKhachHang,TrangThai,NgayGui,NgayHen,NgayTra,DiaChiNhanTraSanPham,KhoangCach,PhiVanChuyen,NgayThanhToan,PhuongThucThanhToan,TongTien,GhiChu")] PhieuSuaChua phieuSuaChua)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(phieuSuaChua);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuSuaChua.KhachHangId);
        //    ViewData["KyThuatId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuSuaChua.KyThuatId);
        //    ViewData["PhuongId"] = new SelectList(_context.Phuongs, "PhuongId", "PhuongId", phieuSuaChua.PhuongId);
        //    return View(phieuSuaChua);
        //}

        // GET: QuanTriVien/AssignTasks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
            if (phieuSuaChua == null)
            {
                return NotFound();
            }

            var assignedIds = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId != null && p.PhieuSuaChuaId != id)
                .Select(p => p.KyThuatId)
                .ToListAsync();

            ViewBag.AssignedTechnicians = await _context.NguoiDungs
                .Where(u => assignedIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.UnassignedTechnicians = await _context.NguoiDungs
                .Where(u => u.VaiTro == "KyThuatVien" && !assignedIds.Contains(u.Id))
                .ToListAsync();

            ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "KhachHang"), "Id", "Id", phieuSuaChua.KhachHangId);
            ViewData["KyThuatId"] = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "KyThuatVien"), "Id", "Id", phieuSuaChua.KyThuatId);
            ViewData["PhuongId"] = new SelectList(_context.Phuongs, "PhuongId", "PhuongId", phieuSuaChua.PhuongId);
            return View(phieuSuaChua);
        }

        // POST: QuanTriVien/AssignTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhieuSuaChuaId,KyThuatId,TrangThai")] PhieuSuaChua phieuSuaChua)
        {
            if (id != phieuSuaChua.PhieuSuaChuaId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(phieuSuaChua.PhuongId));
            ModelState.Remove(nameof(phieuSuaChua.KhachHangId));
            ModelState.Remove(nameof(phieuSuaChua.MoTaKhachHang));
            ModelState.Remove(nameof(phieuSuaChua.DiaChiNhanTraSanPham));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPhieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
                    if (existingPhieuSuaChua == null)
                    {
                        return NotFound();
                    }
                    // Cập nhật các thuộc tính cần thiết
                    existingPhieuSuaChua.KyThuatId = phieuSuaChua.KyThuatId;

                    if (!string.IsNullOrEmpty(phieuSuaChua.KyThuatId))
                    {
                        existingPhieuSuaChua.TrangThai = TrangThaiPhieu.DaPhanCong;
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuSuaChuaExists(phieuSuaChua.PhieuSuaChuaId))
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

            var assignedIds = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId != null && p.PhieuSuaChuaId != id)
                .Select(p => p.KyThuatId)
                .ToListAsync();

            ViewBag.AssignedTechnicians = await _context.NguoiDungs
                .Where(u => assignedIds.Contains(u.Id))
                .ToListAsync();

            ViewBag.UnassignedTechnicians = await _context.NguoiDungs
                .Where(u => u.VaiTro == "KyThuatVien" && !assignedIds.Contains(u.Id))
                .ToListAsync();

            ViewData["KhachHangId"] = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "KhachHang"), "Id", "Id", phieuSuaChua.KhachHangId);
            ViewData["KyThuatId"] = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "KyThuatVien"), "Id", "Id", phieuSuaChua.KyThuatId);
            ViewData["PhuongId"] = new SelectList(_context.Phuongs, "PhuongId", "PhuongId", phieuSuaChua.PhuongId);
            return View(phieuSuaChua);
        }


        //QUANTRIVIEN KHONG XOA PHIEUSUACHUA

        //// GET: QuanTriVien/AssignTasks/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var phieuSuaChua = await _context.PhieuSuaChuas
        //        .Include(p => p.KhachHang)
        //        .Include(p => p.KyThuat)
        //        .Include(p => p.Phuong)
        //        .FirstOrDefaultAsync(m => m.PhieuSuaChuaId == id);
        //    if (phieuSuaChua == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(phieuSuaChua);
        //}

        //// POST: QuanTriVien/AssignTasks/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var phieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
        //    if (phieuSuaChua != null)
        //    {
        //        _context.PhieuSuaChuas.Remove(phieuSuaChua);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool PhieuSuaChuaExists(string id)
        {
            return _context.PhieuSuaChuas.Any(e => e.PhieuSuaChuaId == id);
        }
    }
}
