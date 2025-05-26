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
    public class PhieuNhapsController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;

        public PhieuNhapsController(BHSC_DbContext context, IDGenerator idGenerator)
        {
            _context = context;
            _idGenerator = idGenerator;
        }

        // GET: NhanVienKho/PhieuNhaps
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            int pageSize = 10;
            var phieuNhapQuery = _context.PhieuNhaps.AsQueryable();

            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                phieuNhapQuery = phieuNhapQuery.Where(pn =>
                    pn.PhieuNhapId.Contains(searchString) ||
                    pn.KhoId.Contains(searchString));
            }

            phieuNhapQuery = phieuNhapQuery.OrderBy(pn => pn.KhoId);

            // Đảm bảo trả về đúng kiểu model
            return View(await PaginatedList<PhieuNhap>.CreateAsync(
                phieuNhapQuery.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //GET: NhanVienKho/PhieuNhaps/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var phieuNhap = await _context.PhieuNhaps
                .Include(pn => pn.ChiTietPns) 
                    .ThenInclude(ct => ct.LinhKien)
                .FirstOrDefaultAsync(pn => pn.PhieuNhapId == id);

            if (phieuNhap == null)
                return NotFound();

            return View(phieuNhap);
        }


        // GET: NhanVienKho/PhieuNhaps/Create
        public IActionResult Create()
        {
            var userName = User.Identity?.Name;
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(x => x.UserName == userName);

            if (nguoiDung == null)
            {
                ModelState.AddModelError("", "Không tìm thấy mã kho hợp lệ cho tài khoản này.");
                return View();
            }

            var linhKienList = _context.LinhKiens
                .Select(p => new LinhKienInputModel
                {
                    LinhKienId = p.LinhKienId,
                    TenLinhKien = p.TenLinhKien,
                    DonGia = p.DonGia,
                    SoLuong = 0
                }).ToList();

            var model = new PhieuNhapCreateViewModel
            {
                PhieuNhap = new PhieuNhap
                {
                    KhoId = nguoiDung.Id,
                    NgayNhap = DateTime.Now
                },
                LinhKienInputs = linhKienList
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhapCreateViewModel viewModel)
        {
            var userName = User.Identity?.Name;
            var nguoiDung = _context.NguoiDungs.FirstOrDefault(x => x.UserName == userName);

            if (nguoiDung == null)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View(viewModel);
            }

            var phieuNhap = viewModel.PhieuNhap;

            if (string.IsNullOrEmpty(phieuNhap.PhieuNhapId))
                phieuNhap.PhieuNhapId = await _idGenerator.GeneratePhieuNhapIdAsync();

            phieuNhap.KhoId = nguoiDung.Id;
            phieuNhap.NgayNhap = DateTime.Now;
            phieuNhap.TongTien = 0;

            if (ModelState.IsValid)
            {
                _context.PhieuNhaps.Add(phieuNhap);
                await _context.SaveChangesAsync();

                foreach (var input in viewModel.LinhKienInputs)
                {
                    if (input.SoLuong > 0)
                    {
                        var linhKien = await _context.LinhKiens.FindAsync(input.LinhKienId);
                        if (linhKien != null)
                        {
                            var chiTiet = new ChiTietPn
                            {
                                PhieuNhapId = phieuNhap.PhieuNhapId,
                                LinhKienId = input.LinhKienId,
                                SoLuong = input.SoLuong
                            };

                            _context.ChiTietPns.Add(chiTiet);
                            phieuNhap.TongTien += input.SoLuong * linhKien.DonGia;

                            // Cộng thêm số lượng tồn cho linh kiện
                            linhKien.SoLuongTon += input.SoLuong;
                            _context.LinhKiens.Update(linhKien);
                        }
                    }
                }

                _context.PhieuNhaps.Update(phieuNhap);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }




        // GET: NhanVienKho/PhieuNhaps/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var phieuNhap = await _context.PhieuNhaps
                .Include(pn => pn.ChiTietPns)
                .FirstOrDefaultAsync(pn => pn.PhieuNhapId == id);

            if (phieuNhap == null)
                return NotFound();

            // Get all LinhKien
            var allLinhKien = await _context.LinhKiens.ToListAsync();

            // Build LinhKienInputs with SoLuong from existing ChiTietPn if available
            var linhKienInputs = allLinhKien.Select(lk =>
            {
                var chiTiet = phieuNhap.ChiTietPns.FirstOrDefault(ct => ct.LinhKienId == lk.LinhKienId);
                return new LinhKienInputModel
                {
                    LinhKienId = lk.LinhKienId,
                    TenLinhKien = lk.TenLinhKien,
                    DonGia = lk.DonGia,
                    SoLuong = chiTiet?.SoLuong ?? 0
                };
            }).ToList();

            var viewModel = new PhieuNhapCreateViewModel
            {
                PhieuNhap = phieuNhap,
                LinhKienInputs = linhKienInputs
            };

            return View(viewModel);
        }



        // POST: NhanVienKho/PhieuNhaps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PhieuNhapCreateViewModel viewModel)
        {
            if (id != viewModel.PhieuNhap.PhieuNhapId)
                return NotFound();

            var phieuNhap = await _context.PhieuNhaps
                .Include(pn => pn.ChiTietPns)
                .FirstOrDefaultAsync(pn => pn.PhieuNhapId == id);

            if (phieuNhap == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // 1. Cộng lại số lượng tồn cho các linh kiện từ chi tiết phiếu nhập cũ
                foreach (var oldDetail in phieuNhap.ChiTietPns)
                {
                    var linhKien = await _context.LinhKiens.FindAsync(oldDetail.LinhKienId);
                    if (linhKien != null)
                    {
                        linhKien.SoLuongTon -= oldDetail.SoLuong;
                        if (linhKien.SoLuongTon < 0) linhKien.SoLuongTon = 0;
                        _context.LinhKiens.Update(linhKien);
                    }
                }

                // 2. Xóa chi tiết phiếu nhập cũ
                _context.ChiTietPns.RemoveRange(phieuNhap.ChiTietPns);

                // 3. Thêm chi tiết phiếu nhập mới và cộng lại số lượng tồn
                phieuNhap.TongTien = 0;
                phieuNhap.TrangThai = viewModel.PhieuNhap.TrangThai;
                phieuNhap.GhiChu = viewModel.PhieuNhap.GhiChu;
                phieuNhap.NgayNhap = DateTime.Now;

                foreach (var input in viewModel.LinhKienInputs)
                {
                    if (input.SoLuong > 0)
                    {
                        var linhKien = await _context.LinhKiens.FindAsync(input.LinhKienId);
                        if (linhKien != null)
                        {
                            var chiTiet = new ChiTietPn
                            {
                                PhieuNhapId = phieuNhap.PhieuNhapId,
                                LinhKienId = input.LinhKienId,
                                SoLuong = input.SoLuong
                            };
                            _context.ChiTietPns.Add(chiTiet);
                            phieuNhap.TongTien += input.SoLuong * linhKien.DonGia;

                            // Cộng lại số lượng tồn mới
                            linhKien.SoLuongTon += input.SoLuong;
                            _context.LinhKiens.Update(linhKien);
                        }
                    }
                }

                _context.PhieuNhaps.Update(phieuNhap);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload LinhKienInputs if ModelState is invalid
            var allLinhKien = await _context.LinhKiens.ToListAsync();
            viewModel.LinhKienInputs = allLinhKien.Select(lk =>
            {
                var input = viewModel.LinhKienInputs.FirstOrDefault(x => x.LinhKienId == lk.LinhKienId);
                return new LinhKienInputModel
                {
                    LinhKienId = lk.LinhKienId,
                    TenLinhKien = lk.TenLinhKien,
                    DonGia = lk.DonGia,
                    SoLuong = input?.SoLuong ?? 0
                };
            }).ToList();

            return View(viewModel);
        }





        // GET: NhanVienKho/PhieuNhaps/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuNhap = await _context.PhieuNhaps
                .Include(p => p.Kho)
                .FirstOrDefaultAsync(m => m.PhieuNhapId == id);
            if (phieuNhap == null)
            {
                return NotFound();
            }

            return View(phieuNhap);
        }

        // POST: NhanVienKho/PhieuNhaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Xóa toàn bộ chi tiết phiếu nhập liên quan
            var chiTietList = await _context.ChiTietPns
                .Where(x => x.PhieuNhapId == id)
                .ToListAsync();

            if (chiTietList.Any())
            {
                _context.ChiTietPns.RemoveRange(chiTietList);
            }

            // Xóa phiếu xuất
            var phieuNhap = await _context.PhieuNhaps.FindAsync(id);
            if (phieuNhap != null)
            {
                _context.PhieuNhaps.Remove(phieuNhap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
