using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    public class DuyetDonController : Controller
    {
        private readonly BHSC_DbContext _context;

        public DuyetDonController(BHSC_DbContext context)
        {
            _context = context;
        }

        // Danh sách đơn chưa duyệt
        public IActionResult DanhSachDon()
        {
            var danhSach = _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
              .Where(p => p.TrangThai.ToLower() == "ChuaDuyet")
                .ToList();

            return View(danhSach);
        }

        // Chi tiết đơn
        public IActionResult ChiTietDon(int id)
        {
            var phieu = _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(ct => ct.LinhKien)
                .FirstOrDefault(p => p.PhieuSuaChuaId == id);

            if (phieu == null)
                return NotFound();

            return View(phieu);
        }

        // Duyet don
        [HttpPost]
        public IActionResult Duyet(int id)
        {
            var phieu = _context.PhieuSuaChuas.Find(id);
            if (phieu != null)
            {
                phieu.TrangThai = "DaDuyet";
                _context.SaveChanges();
            }
            return RedirectToAction("DanhSachDon");
        }
    }
}
