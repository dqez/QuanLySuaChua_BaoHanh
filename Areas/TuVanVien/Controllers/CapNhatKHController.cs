using Microsoft.AspNetCore.Mvc;
using QuanLySuaChua_BaoHanh.Models;
using Microsoft.EntityFrameworkCore;

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

        // Hiển thị danh sách khách hàng
        public IActionResult CapNhatKhachHang(string? searchId)
        {
            var khachHangs = _context.NguoiDungs
                .Where(nd => nd.VaiTro == "KhachHang");

            if (!string.IsNullOrEmpty(searchId) && int.TryParse(searchId, out int id))
            {
                khachHangs = khachHangs.Where(nd => nd.Id == id);
            }

            return View(khachHangs.ToList());
        }

        // Hiển thị form chỉnh sửa
        public IActionResult Edit(int id)
        {
            var kh = _context.NguoiDungs.FirstOrDefault(nd => nd.Id == id && nd.VaiTro == "KhachHang");
            if (kh == null)
                return NotFound();

            return View("Sua", kh);
        }

        // Xóa 1 khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Xoa(int id)
        {
            var kh = _context.NguoiDungs.Find(id);
            if (kh != null)
            {
                _context.NguoiDungs.Remove(kh);
                _context.SaveChanges();
            }
            return RedirectToAction("CapNhatKhachHang");
        }

        // Xóa nhiều khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaNhieu(List<int> selectedIds)
        {
            if (selectedIds != null && selectedIds.Count > 0)
            {
                var khachHangs = _context.NguoiDungs
                    .Where(nd => selectedIds.Contains(nd.Id) && nd.VaiTro == "KhachHang")
                    .ToList();

                _context.NguoiDungs.RemoveRange(khachHangs);
                _context.SaveChanges();
            }

            return RedirectToAction("CapNhatKhachHang");
        }
    }
}
