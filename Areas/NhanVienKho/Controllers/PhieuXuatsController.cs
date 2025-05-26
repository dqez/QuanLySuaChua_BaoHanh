using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Models;
using QuanLySuaChua_BaoHanh.Enums;
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
                return NotFound();

            var phieuXuat = await _context.PhieuXuats
                .Include(px => px.ChiTietPxes) // load chi tiết phiếu xuất
                    .ThenInclude(ct => ct.PhieuSuaChua) // load phiếu sửa chữa trong chi tiết phiếu xuất
                .FirstOrDefaultAsync(px => px.PhieuXuatId == id);

            if (phieuXuat == null)
                return NotFound();

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

            // Lấy toàn bộ Phiếu Sửa Chữa
            var phieuSuaChuaList = _context.PhieuSuaChuas
                .Where(p => p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString() && p.TongTien != null)
                .Select(p => new SelectListItem
                {
                    Value = p.PhieuSuaChuaId.ToString(),
                    Text = "Phiếu #" + p.PhieuSuaChuaId
                })
                .ToList();

            ViewBag.PhieuSuaChuaList = phieuSuaChuaList;

            var model = new PhieuXuat
            {
                KhoId = nguoiDung.Id,
                NgayXuat = DateTime.Now
            };

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

                    // Trừ số lượng tồn linh kiện theo từng chi tiết sửa chữa
                    var chiTietSuaChuas = await _context.ChiTietSuaChuas
                        .Where(ct => ct.PhieuSuaChuaId == phieuSuaChuaId && ct.LinhKienId != null)
                        .ToListAsync();

                    foreach (var chiTiet in chiTietSuaChuas)
                    {
                        var linhKien = await _context.LinhKiens.FindAsync(chiTiet.LinhKienId);
                        if (linhKien != null)
                        {
                            linhKien.SoLuongTon -= chiTiet.SoLuongLinhKien;
                            if (linhKien.SoLuongTon < 0) linhKien.SoLuongTon = 0;
                            _context.LinhKiens.Update(linhKien);
                        }
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

            // Populate ViewBag.PhieuSuaChuaList
            var phieuSuaChuaList = _context.PhieuSuaChuas
                .Select(p => new SelectListItem
                {
                    Value = p.PhieuSuaChuaId.ToString(),
                    Text = "Phiếu #" + p.PhieuSuaChuaId
                })
                .ToList();
            ViewBag.PhieuSuaChuaList = phieuSuaChuaList;

            ViewData["KhoId"] = new SelectList(_context.NguoiDungs, "Id", "Id", phieuXuat.KhoId);
            return View(phieuXuat);
        }


        // POST: NhanVienKho/PhieuXuats/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string[] phieuSuaChuaIds, [Bind("PhieuXuatId,KhoId,NgayXuat,TongTien,TrangThai,GhiChu")] PhieuXuat phieuXuat)
        {
            if (id != phieuXuat.PhieuXuatId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy các chi tiết phiếu xuất cũ
                    var chiTietCu = await _context.ChiTietPxes
                        .Where(c => c.PhieuXuatId == id)
                        .ToListAsync();

                    // Cộng lại số lượng tồn cho các linh kiện đã xuất ở chi tiết cũ
                    foreach (var ctpx in chiTietCu)
                    {
                        var chiTietSuaChuas = await _context.ChiTietSuaChuas
                            .Where(ct => ct.PhieuSuaChuaId == ctpx.PhieuSuaChuaId && ct.LinhKienId != null)
                            .ToListAsync();

                        foreach (var chiTiet in chiTietSuaChuas)
                        {
                            var linhKien = await _context.LinhKiens.FindAsync(chiTiet.LinhKienId);
                            if (linhKien != null)
                            {
                                linhKien.SoLuongTon += chiTiet.SoLuongLinhKien;
                                _context.LinhKiens.Update(linhKien);
                            }
                        }
                    }

                    // Xóa chi tiết cũ
                    _context.ChiTietPxes.RemoveRange(chiTietCu);
                    await _context.SaveChangesAsync();

                    // Reset tổng tiền
                    phieuXuat.TongTien = 0;

                    // Thêm chi tiết mới và trừ lại số lượng tồn
                    foreach (var phieuSuaChuaId in phieuSuaChuaIds)
                    {
                        var ctpx = new ChiTietPx
                        {
                            PhieuXuatId = id,
                            PhieuSuaChuaId = phieuSuaChuaId,
                            GhiChu = phieuXuat.GhiChu
                        };
                        _context.ChiTietPxes.Add(ctpx);

                        var psc = await _context.PhieuSuaChuas.FindAsync(phieuSuaChuaId);
                        if (psc != null && psc.TongTien.HasValue)
                        {
                            phieuXuat.TongTien += psc.TongTien.Value;
                        }

                        // Trừ số lượng tồn linh kiện theo từng chi tiết sửa chữa
                        var chiTietSuaChuas = await _context.ChiTietSuaChuas
                            .Where(ct => ct.PhieuSuaChuaId == phieuSuaChuaId && ct.LinhKienId != null)
                            .ToListAsync();

                        foreach (var chiTiet in chiTietSuaChuas)
                        {
                            var linhKien = await _context.LinhKiens.FindAsync(chiTiet.LinhKienId);
                            if (linhKien != null)
                            {
                                linhKien.SoLuongTon -= chiTiet.SoLuongLinhKien;
                                if (linhKien.SoLuongTon < 0) linhKien.SoLuongTon = 0;
                                _context.LinhKiens.Update(linhKien);
                            }
                        }
                    }

                    // Cập nhật lại phiếu xuất
                    _context.Update(phieuXuat);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PhieuXuats.Any(e => e.PhieuXuatId == phieuXuat.PhieuXuatId))
                        return NotFound();
                    else
                        throw;
                }
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
            // Xóa toàn bộ chi tiết phiếu xuất liên quan
            var chiTietList = await _context.ChiTietPxes
                .Where(x => x.PhieuXuatId == id)
                .ToListAsync();

            if (chiTietList.Any())
            {
                _context.ChiTietPxes.RemoveRange(chiTietList);
            }

            // Xóa phiếu xuất
            var phieuXuat = await _context.PhieuXuats.FindAsync(id);
            if (phieuXuat != null)
            {
                _context.PhieuXuats.Remove(phieuXuat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
