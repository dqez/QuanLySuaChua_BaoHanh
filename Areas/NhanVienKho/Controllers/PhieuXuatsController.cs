using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Models;
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
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;
            var phieuXuatQuery = _context.PhieuXuats.AsQueryable();

            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                phieuXuatQuery = phieuXuatQuery.Where(px =>
                    px.PhieuXuatId.Contains(searchString) ||
                    px.KhoId.Contains(searchString));
            }

            phieuXuatQuery = phieuXuatQuery.OrderBy(px => px.KhoId);

            return View(await PaginatedList<PhieuXuat>.CreateAsync(
                phieuXuatQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: NhanVienKho/PhieuXuats/Details/5
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
            var userName = User.Identity?.Name;
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(x => x.UserName == userName);

            if (nguoiDung == null)
            {
                ModelState.AddModelError("", "Không tìm thấy mã kho hợp lệ cho tài khoản này.");
                return View();
            }

            var model = new PhieuXuat
            {
                KhoId = nguoiDung.Id,
                NgayXuat = DateTime.Now,
            };

            // Lấy danh sách phiếu sửa chữa có thể xuất
            ViewBag.PhieuSuaChuaList = _context.PhieuSuaChuas
                .Where(p => p.TrangThai == "HoanThanh") // ví dụ
                .Select(p => p.PhieuSuaChuaId)
                .ToArray();

            return View(model);
        }


        // POST: NhanVienKho/PhieuXuats/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] phieuSuaChuaIds, [Bind("PhieuXuatId,KhoId,NgayXuat,TongTien,TrangThai,GhiChu")] PhieuXuat phieuXuat)
        {
            var userName = User.Identity?.Name;
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(x => x.UserName == userName);

            // Sinh mã phiếu xuất nếu cần
            if (string.IsNullOrEmpty(phieuXuat.PhieuXuatId))
                phieuXuat.PhieuXuatId = await _idGenerator.GeneratePhieuXuatIdAsync();

            phieuXuat.KhoId = nguoiDung.Id;
            phieuXuat.NgayXuat = DateTime.Now;
            phieuXuat.TongTien = 0;
            phieuXuat.TrangThai = "...";

            if (ModelState.IsValid)
            {
                // Thêm phiếu xuất trước để có PhieuXuatId
                _context.Add(phieuXuat);
                await _context.SaveChangesAsync();

                // Thêm các chi tiết phiếu xuất
                foreach (var phieuSuaChuaId in phieuSuaChuaIds)
                {
                    var ctpx = new ChiTietPx
                    {
                        PhieuXuatId = phieuXuat.PhieuXuatId,
                        PhieuSuaChuaId = phieuSuaChuaId,
                        GhiChu = phieuXuat.GhiChu
                    };
                    _context.ChiTietPxes.Add(ctpx);

                    // Tính tổng tiền cho phiếu xuất
                    var phieu = await _context.PhieuSuaChuas.FindAsync(phieuSuaChuaId);
                    if (phieu != null && phieu.TongTien.HasValue)
                    {
                        phieuXuat.TongTien += phieu.TongTien.Value;
                    }
                }

                // Cập nhật tổng tiền sau khi thêm chi tiết
                _context.Update(phieuXuat);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, trả lại view với model
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
