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
    public class ChiTietPnsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public ChiTietPnsController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }

        // GET: NhanVienKho/ChiTietPns
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;
            var chiTietPnsQuerry = _context.ChiTietPns.AsQueryable();

            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                chiTietPnsQuerry = chiTietPnsQuerry.Where(ctpn =>
                    ctpn.PhieuNhapId.Contains(searchString) ||
                    ctpn.LinhKienId.Contains(searchString));
            }

            chiTietPnsQuerry = chiTietPnsQuerry.OrderBy(ctpn => ctpn.PhieuNhapId);

            return View(await PaginatedList<ChiTietPn>.CreateAsync(
                chiTietPnsQuerry.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: NhanVienKho/ChiTietPns/Create
        public IActionResult Create(string phieuNhapId)
        {
            ViewBag.PhieuNhapId = phieuNhapId;
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "TenLinhKien");
            // Lấy danh sách chi tiết đã nhập
            var chiTietList = _context.ChiTietPns
                .Include(x => x.LinhKien)
                .Where(x => x.PhieuNhapId == phieuNhapId)
                .ToList();
            ViewBag.ChiTietList = chiTietList;
            // Lấy tổng tiền
            var tongTien = chiTietList.Sum(x => x.SoLuong * x.LinhKien.DonGia);
            ViewBag.TongTien = tongTien;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAfterChiTiet(string phieuNhapId, decimal tongTien)
        {
            var existingPhieuNhap = await _context.PhieuNhaps.FindAsync(phieuNhapId);
            if (existingPhieuNhap == null)
            {
                // Chỉ tạo mới nếu chưa tồn tại
                var newPhieuNhap = new PhieuNhap
                {
                    PhieuNhapId = phieuNhapId,
                    NgayNhap = DateTime.Now,
                    TongTien = tongTien
                };
                _context.PhieuNhaps.Add(newPhieuNhap);
            }
            else
            {
                // Nếu đã tồn tại thì cập nhật lại tổng tiền
                existingPhieuNhap.TongTien = tongTien;
                _context.PhieuNhaps.Update(existingPhieuNhap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // hoặc trang danh sách phiếu nhập
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhieuNhapId,LinhKienId,SoLuong")] ChiTietPn chiTietPn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietPn);
                await _context.SaveChangesAsync();

                // Cập nhật tổng tiền cho phiếu nhập
                var tongTien = _context.ChiTietPns
                    .Where(x => x.PhieuNhapId == chiTietPn.PhieuNhapId)
                    .Sum(x => x.SoLuong * x.LinhKien.DonGia);

                var phieuNhap = await _context.PhieuNhaps.FindAsync(chiTietPn.PhieuNhapId);
                if (phieuNhap != null)
                {
                    phieuNhap.TongTien = tongTien;
                    await _context.SaveChangesAsync();
                }

                // Quay lại trang tạo chi tiết để nhập tiếp
                return RedirectToAction("Create", new { phieuNhapId = chiTietPn.PhieuNhapId });
            }
            // Nếu lỗi, load lại dữ liệu
            ViewBag.PhieuNhapId = chiTietPn.PhieuNhapId;
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "TenLinhKien", chiTietPn.LinhKienId);
            ViewBag.ChiTietList = _context.ChiTietPns
                .Include(x => x.LinhKien)
                .Where(x => x.PhieuNhapId == chiTietPn.PhieuNhapId)
                .ToList();
            return View(chiTietPn);
        }
        public async Task<IActionResult> HoanTatPhieuNhap(string phieuNhapId)
        {
            var chiTietList = await _context.ChiTietPns
                .Include(x => x.LinhKien)
                .Where(x => x.PhieuNhapId == phieuNhapId)
                .ToListAsync();

            if (!chiTietList.Any())
            {
                TempData["Error"] = "Bạn chưa nhập chi tiết nào!";
                return RedirectToAction("Create", new { phieuNhapId });
            }

            var tongTien = chiTietList.Sum(x => x.SoLuong * x.LinhKien.DonGia);

            // Kiểm tra xem đã tồn tại chưa
            var existingPhieu = await _context.PhieuNhaps.FindAsync(phieuNhapId);
            if (existingPhieu == null)
            {
                var phieu = new PhieuNhap
                {
                    PhieuNhapId = phieuNhapId,
                    NgayNhap = DateTime.Now,
                    TongTien = tongTien
                };

                _context.PhieuNhaps.Add(phieu);
                await _context.SaveChangesAsync();
            }
            else
            {
                existingPhieu.TongTien = tongTien;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Tạo phiếu nhập thành công!";
            return RedirectToAction("Index", "PhieuNhaps");
        }     


        // GET: NhanVienKho/ChiTietPns/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPn = await _context.ChiTietPns.FindAsync(id);
            if (chiTietPn == null)
            {
                return NotFound();
            }
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "LinhKienId", chiTietPn.LinhKienId);
            ViewData["PhieuNhapId"] = new SelectList(_context.PhieuNhaps, "PhieuNhapId", "PhieuNhapId", chiTietPn.PhieuNhapId);
            return View(chiTietPn);
        }

        // POST: NhanVienKho/ChiTietPns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PhieuNhapId,LinhKienId,SoLuong")] ChiTietPn chiTietPn)
        {
            if (id != chiTietPn.PhieuNhapId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chiTietPn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietPnExists(chiTietPn.PhieuNhapId))
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
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "LinhKienId", chiTietPn.LinhKienId);
            ViewData["PhieuNhapId"] = new SelectList(_context.PhieuNhaps, "PhieuNhapId", "PhieuNhapId", chiTietPn.PhieuNhapId);
            return View(chiTietPn);
        }

        // GET: NhanVienKho/ChiTietPns/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPn = await _context.ChiTietPns
                .Include(c => c.LinhKien)
                .Include(c => c.PhieuNhap)
                .FirstOrDefaultAsync(m => m.PhieuNhapId == id);
            if (chiTietPn == null)
            {
                return NotFound();
            }

            return View(chiTietPn);
        }

        // POST: NhanVienKho/ChiTietPns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var chiTietPn = await _context.ChiTietPns.FindAsync(id);
            if (chiTietPn != null)
            {
                _context.ChiTietPns.Remove(chiTietPn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChiTietPnExists(string id)
        {
            return _context.ChiTietPns.Any(e => e.PhieuNhapId == id);
        }
    }
}
