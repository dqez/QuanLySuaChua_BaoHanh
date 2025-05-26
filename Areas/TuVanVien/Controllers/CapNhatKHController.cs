using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    [Authorize(Roles = "TuVanVien")]
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
                .AsNoTracking() // ✅ DÙNG cho danh sách
                .Where(nd => nd.VaiTro == "KhachHang");

            if (!string.IsNullOrEmpty(searchId))
            {
                khachHangs = khachHangs.Where(nd => nd.Id == searchId);
            }

            return View(khachHangs.ToList());
        }

        // ❗ KHÔNG dùng AsNoTracking ở đây
        public IActionResult Edit(string id)
        {
            var kh = _context.NguoiDungs
                .FirstOrDefault(nd => nd.Id == id && nd.VaiTro == "KhachHang");

            if (kh == null)
                return NotFound();

            return View("Sua", kh);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NguoiDung model)
        {
            var nguoiDungId = Request.Form["NguoiDungID"].ToString();

            Console.WriteLine("✅ Nhận ID từ form: " + nguoiDungId);

            if (string.IsNullOrEmpty(nguoiDungId))
                return BadRequest("ID không hợp lệ");

            var kh = _context.NguoiDungs.FirstOrDefault(nd => nd.Id == nguoiDungId && nd.VaiTro == "KhachHang");

            if (kh == null)
                return NotFound("Không tìm thấy người dùng");

            kh.HoTen = model.HoTen;
            kh.Email = model.Email;
            kh.PhoneNumber = model.PhoneNumber;
            kh.DiaChi = model.DiaChi;
            kh.PhuongId = model.PhuongId;

            _context.SaveChanges();

            return RedirectToAction("CapNhatKhachHang");
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
                    .Where(nd => selectedIds.Contains(nd.Id) && nd.VaiTro == "KhachHang")
                    .ToList();

                _context.NguoiDungs.RemoveRange(khachHangs);
                _context.SaveChanges();
            }
            return RedirectToAction("CapNhatKhachHang");
        }
    }
}
