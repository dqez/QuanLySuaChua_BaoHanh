using Microsoft.AspNetCore.Mvc;
using QuanLySuaChua_BaoHanh.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    public class CapNhatKHController : Controller
    {
        private readonly BHSC_DbContext _context;

        public CapNhatKHController(BHSC_DbContext context)
        {
            _context = context;
        }

        public IActionResult CapNhatKhachHang(string? searchId)
        {
            var khachHangs = _context.NguoiDungs
                .Where(nd => nd.VaiTro == "KhachHang");

            if (!string.IsNullOrEmpty(searchId))
            {
                khachHangs = khachHangs.Where(nd => nd.Id.Equals(searchId));
            }

            return View(khachHangs.ToList());
        }

        public IActionResult Edit(string id)
        {
            var kh = _context.NguoiDungs.FirstOrDefault(nd => nd.Id.Equals(id) && nd.VaiTro == "KhachHang");
            if (kh == null)
                return NotFound();

            return View("Sua", kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Xoa(string id)
        {
            var kh = _context.NguoiDungs.Find(id);
            if (kh != null)
            {
                _context.NguoiDungs.Remove(kh);
                _context.SaveChanges();
            }
            return RedirectToAction("CapNhatKhachHang");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaNhieu(List<string> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                var khachHangs = _context.NguoiDungs
                   .Where(nd => selectedIds.Contains(nd.Id.ToString()) && nd.VaiTro == "KhachHang")
                    .ToList();

                _context.NguoiDungs.RemoveRange(khachHangs);
                _context.SaveChanges();
            }
            return RedirectToAction("CapNhatKhachHang");
        }
    }
}
