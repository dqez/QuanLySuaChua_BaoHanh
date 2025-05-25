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
            ViewData["PhieuNhapId"] = new SelectList(_context.PhieuNhaps, "PhieuNhapId", "PhieuNhapId");
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "TenLinhKien");
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveAfterChiTiet(string phieuNhapId, decimal tongTien)
        //{
        //    var existingPhieuNhap = await _context.PhieuNhaps.FindAsync(phieuNhapId);
        //    if (existingPhieuNhap == null)
        //    {
        //        // Chỉ tạo mới nếu chưa tồn tại
        //        var newPhieuNhap = new PhieuNhap
        //        {
        //            PhieuNhapId = phieuNhapId,
        //            NgayNhap = DateTime.Now,
        //            TongTien = tongTien
        //        };
        //        _context.PhieuNhaps.Add(newPhieuNhap);
        //    }
        //    else
        //    {
        //        // Nếu đã tồn tại thì cập nhật lại tổng tiền
        //        existingPhieuNhap.TongTien = tongTien;
        //        _context.PhieuNhaps.Update(existingPhieuNhap);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction("Index"); // hoặc trang danh sách phiếu nhập
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhieuNhapId,LinhKienId,SoLuong")] ChiTietPn chiTietPn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chiTietPn);

                // Tăng số lượng tồn cho linh kiện
                var linhKien = await _context.LinhKiens.FindAsync(chiTietPn.LinhKienId);
                if (linhKien != null)
                {
                    linhKien.SoLuongTon += chiTietPn.SoLuong;

                    // Nếu cần kiểm tra điều kiện tồn kho, ví dụ không vượt quá 10000
                    // if (linhKien.SoLuongTon > 10000)
                    // {
                    //     ModelState.AddModelError("", "Số lượng tồn vượt quá giới hạn.");
                    //     return View(chiTietPn);
                    // }
                }

                await _context.SaveChangesAsync();

                // Cập nhật lại tổng tiền cho phiếu nhập
                var tongTien = _context.ChiTietPns
                    .Where(ct => ct.PhieuNhapId == chiTietPn.PhieuNhapId)
                    .Sum(ct => ct.SoLuong * ct.LinhKien.DonGia);

                var phieuNhap = await _context.PhieuNhaps.FindAsync(chiTietPn.PhieuNhapId);
                if (phieuNhap != null)
                {
                    phieuNhap.TongTien = tongTien;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["PhieuNhapId"] = new SelectList(_context.PhieuNhaps, "PhieuNhapId", "PhieuNhapId");
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "TenLinhKien");
            return View(chiTietPn);
        }

        //public async Task<IActionResult> HoanTatPhieuNhap(string phieuNhapId)
        //{
        //    var chiTietList = await _context.ChiTietPns
        //        .Include(x => x.LinhKien)
        //        .Where(x => x.PhieuNhapId == phieuNhapId)
        //        .ToListAsync();

        //    if (!chiTietList.Any())
        //    {
        //        TempData["Error"] = "Bạn chưa nhập chi tiết nào!";
        //        return RedirectToAction("Create", new { phieuNhapId });
        //    }

        //    var tongTien = chiTietList.Sum(x => x.SoLuong * x.LinhKien.DonGia);

        //    // Kiểm tra xem đã tồn tại chưa
        //    var existingPhieu = await _context.PhieuNhaps.FindAsync(phieuNhapId);
        //    if (existingPhieu == null)
        //    {
        //        var phieu = new PhieuNhap
        //        {
        //            PhieuNhapId = phieuNhapId,
        //            NgayNhap = DateTime.Now,
        //            TongTien = tongTien
        //        };

        //        _context.PhieuNhaps.Add(phieu);
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //    {
        //        existingPhieu.TongTien = tongTien;
        //        await _context.SaveChangesAsync();
        //    }

        //    TempData["Success"] = "Tạo phiếu nhập thành công!";
        //    return RedirectToAction("Index", "PhieuNhaps");
        //}     


        // GET: NhanVienKho/ChiTietPns/Edit/5
        public async Task<IActionResult> Edit(string phieuNhapId, string linhKienId)
        {
            if (phieuNhapId == null || linhKienId == null)
                return NotFound();

            var chiTietPn = await _context.ChiTietPns
                .FirstOrDefaultAsync(x => x.PhieuNhapId == phieuNhapId && x.LinhKienId == linhKienId);

            if (chiTietPn == null)
                return NotFound();
            ViewData["LinhKienId"] = new SelectList(_context.LinhKiens, "LinhKienId", "LinhKienId", chiTietPn.LinhKienId);
            ViewData["PhieuNhapId"] = new SelectList(_context.PhieuNhaps, "PhieuNhapId", "PhieuNhapId", chiTietPn.PhieuNhapId);
            return View(chiTietPn);
        }

        // POST: NhanVienKho/ChiTietPns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string phieuNhapId, string linhKienId, [Bind("PhieuNhapId,LinhKienId,SoLuong")] ChiTietPn chiTietPn)
        {
            if (phieuNhapId != chiTietPn.PhieuNhapId || linhKienId != chiTietPn.LinhKienId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy chi tiết cũ để tính lại số lượng tồn
                    var oldChiTiet = await _context.ChiTietPns
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.PhieuNhapId == phieuNhapId && x.LinhKienId == linhKienId);

                    if (oldChiTiet != null)
                    {
                        var linhKien = await _context.LinhKiens.FindAsync(linhKienId);
                        if (linhKien != null)
                        {
                            // Trừ số lượng cũ, cộng số lượng mới
                            linhKien.SoLuongTon = linhKien.SoLuongTon - oldChiTiet.SoLuong + chiTietPn.SoLuong;
                            if (linhKien.SoLuongTon < 0) linhKien.SoLuongTon = 0;
                        }
                    }

                    _context.Update(chiTietPn);
                    await _context.SaveChangesAsync();

                    // Cập nhật lại tổng tiền cho phiếu nhập
                    var tongTien = await _context.ChiTietPns
                        .Where(ct => ct.PhieuNhapId == phieuNhapId)
                        .SumAsync(ct => ct.SoLuong * ct.LinhKien.DonGia);

                    var phieuNhap = await _context.PhieuNhaps.FindAsync(phieuNhapId);
                    if (phieuNhap != null)
                    {
                        phieuNhap.TongTien = tongTien;
                        await _context.SaveChangesAsync();
                    }
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


        //// GET: NhanVienKho/ChiTietPns/Delete/5
        //public async Task<IActionResult> Delete(string phieuNhapId, string linhKienId)
        //{
        //    if (phieuNhapId == null || linhKienId == null)
        //        return NotFound();

        //    var chiTietPn = await _context.ChiTietPns
        //        .FirstOrDefaultAsync(x => x.PhieuNhapId == phieuNhapId && x.LinhKienId == linhKienId);

        //    if (chiTietPn == null)
        //        return NotFound();

        //    return View(chiTietPn);
        //}

        // POST: NhanVienKho/ChiTietPns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string phieuNhapId, string linhKienId)
        {
            var chiTietPn = await _context.ChiTietPns
                .Include(x => x.LinhKien)
                .FirstOrDefaultAsync(x => x.PhieuNhapId == phieuNhapId && x.LinhKienId == linhKienId);

            if (chiTietPn != null)
            {
                // Giảm số lượng tồn của linh kiện
                var linhKien = chiTietPn.LinhKien;
                if (linhKien != null)
                {
                    linhKien.SoLuongTon -= chiTietPn.SoLuong;
                    if (linhKien.SoLuongTon < 0) linhKien.SoLuongTon = 0;
                }

                _context.ChiTietPns.Remove(chiTietPn);
                await _context.SaveChangesAsync();

                // Cập nhật lại tổng tiền cho phiếu nhập
                var tongTien = await _context.ChiTietPns
                    .Where(ct => ct.PhieuNhapId == phieuNhapId)
                    .SumAsync(ct => ct.SoLuong * ct.LinhKien.DonGia);

                var phieuNhap = await _context.PhieuNhaps.FindAsync(phieuNhapId);
                if (phieuNhap != null)
                {
                    phieuNhap.TongTien = tongTien;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["Success"] = "Xóa chi tiết phiếu nhập thành công!";
            return RedirectToAction(nameof(Index));
        }


        private bool ChiTietPnExists(string id)
        {
            return _context.ChiTietPns.Any(e => e.PhieuNhapId == id);
        }
    }
}
