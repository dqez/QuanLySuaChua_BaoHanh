using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    [Authorize(Roles = "TuVanVien")]
    public class ThongKeController : Controller
    {
        private readonly BHSC_DbContext _context;

        public ThongKeController(BHSC_DbContext context)
        {
            _context = context;
        }

        private void ValidateAndPrepareDateRange(ref DateTime? tuNgay, ref DateTime? denNgay, int defaultPastDays = 30)
        {
            tuNgay ??= DateTime.Now.AddDays(-defaultPastDays);
            denNgay ??= DateTime.Now;

            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("DateRangeError", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                tuNgay = DateTime.Now.AddDays(-defaultPastDays);
                denNgay = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
        }

        // Trang tổng quan thống kê
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            ViewBag.TongSoDonSuaChua = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .CountAsync();

            ViewBag.TongDoanhThu = await _context.PhieuSuaChuas
                .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                .SumAsync(p => p.TongTien ?? 0);

            ViewBag.SoDonChoXacNhan = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.ChoXacNhan.ToString())
                .CountAsync();

            ViewBag.SoDonDangXuLy = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString())
                .CountAsync();

            ViewBag.SoDonHoanThanh = await _context.PhieuSuaChuas
                .Where(p => p.NgayTra >= tuNgay && p.NgayTra <= denNgay && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                .CountAsync();

            ViewBag.SoDonHuy = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DaHuy.ToString())
                .CountAsync();

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);

            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            ViewBag.TongDoanhThu = await _context.PhieuSuaChuas
                .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                .SumAsync(p => p.TongTien ?? 0);

            ViewBag.SoDonHoanThanh = await _context.PhieuSuaChuas
                .Where(p => p.NgayTra >= tuNgay && p.NgayTra <= denNgay && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                .CountAsync();

            return View(); // Sử dụng View: DoanhThu.cshtml
        }

        [HttpGet]
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> DonSuaChua(DateTime? tuNgay, DateTime? denNgay)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;

            var phieu = _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay);

            ViewBag.TongSoDon = await phieu.CountAsync();

            // Tính thời gian trung bình xử lý (sử dụng client-side để tránh lỗi EF không dịch được)
            var donTra = await phieu
                .Where(p => p.NgayTra.HasValue)
                .Select(p => new { p.NgayGui, NgayTra = p.NgayTra.Value })
                .ToListAsync();

            double thoiGianTrungBinh = donTra.Any()
                ? donTra.Average(p => (p.NgayTra - p.NgayGui).TotalDays)
                : 0;

            ViewBag.ThoiGianTrungBinh = thoiGianTrungBinh;

            // Thống kê trạng thái
            ViewBag.ThongKeTrangThai = await phieu
                .GroupBy(p => p.TrangThai)
                .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            ViewBag.DonTheoNgay = await phieu
     .GroupBy(p => p.NgayGui.Date)
     .Select(g => new { Ngay = g.Key, SoLuongDon = g.Count() })
     .ToListAsync();

            return View();
        }



    }
}
