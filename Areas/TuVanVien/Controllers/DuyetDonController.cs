using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    [Authorize(Roles = "TuVanVien")]
    public class DuyetDonController : Controller
    {
        private readonly BHSC_DbContext _context;

        public DuyetDonController(BHSC_DbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DanhSachDon()
        {
            var danhSach = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SanPham)
                .Where(p => p.TrangThai == "ChuaDuyet")
                .OrderByDescending(p => p.NgayGui)
                .ToListAsync();

            return View(danhSach);
        }
        public async Task<IActionResult> ChiTietDon(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var phieu = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.LinhKien)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.SanPham)
                .Include(p => p.Phuong)
                .FirstOrDefaultAsync(p => p.PhieuSuaChuaId == id);

            if (phieu == null)
                return NotFound();

            return View(phieu);
        }[HttpPost]
        public IActionResult Duyet(string id)
        {
            var phieu = _context.PhieuSuaChuas.Find(id);
            if (phieu != null)
            {
                phieu.TrangThai = QuanLySuaChua_BaoHanh.Enums.TrangThaiPhieu.DaXacNhan.ToString();
                _context.SaveChanges();
            }
            return RedirectToAction("DanhSachDon");
        }
    }
}
