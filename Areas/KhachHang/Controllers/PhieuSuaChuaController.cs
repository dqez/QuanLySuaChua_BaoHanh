using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;
using System.Security.Claims;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize(Roles = "KhachHang")]
    public class PhieuSuaChuaController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly UserManager<NguoiDung> _userManager;

        public PhieuSuaChuaController(BHSC_DbContext context, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: KhachHang/PhieuSuaChua/Create
        public async Task<IActionResult> Create()
        {
            // Get user's products
            var user = _userManager.GetUserAsync(User);
            var Products = await _context.SanPhams
                .Where(p => p.KhachHangId == user.Result.Id)
                .ToListAsync();

            if(Products.Count > 0)
            {
               ViewBag.Products = Products;
            }
            return View();
        }

        // POST: KhachHang/PhieuSuaChua/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuSuaChuaVM phieuSuaChuaVM)
        {
            if (!ModelState.IsValid)
            {
                var user = _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }
                PhieuSuaChua phieuSuaChua = new PhieuSuaChua
                {
                    PhieuSuaChuaId = $"PSC{DateTime.Now:yyyyMMddHHmmss}",
                    KhachHangId = user.Result.Id,
                    NgayGui = DateTime.Now,
                    TrangThai = "Chờ xử lý",
                    MoTaKhachHang = phieuSuaChuaVM.MoTaKhachHang,
                    DiaChiNhanTraSanPham = phieuSuaChuaVM.DiaChiNhanTraSanPham,
                    PhuongId = phieuSuaChuaVM.PhuongId
                };          
                _context.PhieuSuaChuas.Add(phieuSuaChua);

                foreach(var sanPhamId in phieuSuaChuaVM.SanPhamIds)
                {
                    var sanPham = await _context.SanPhams.FindAsync(sanPhamId);
                    if (sanPham != null)
                    {
                        ChiTietSuaChua chiTietSuaChua = new ChiTietSuaChua
                        {
                            ChiTietId = $"CT{DateTime.Now:yyyyMMddHHmmss}",
                            SanPhamId = sanPhamId,
                            PhieuSuaChuaId = phieuSuaChua.PhieuSuaChuaId,
                            LinhKienId= null,
                            LoaiDon = sanPham.NgayHetHanBh > DateOnly.FromDateTime(DateTime.Now) ? "Sửa chưa" : "Bảo hành",
                        };
                    }
                }
              
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đăng ký phiếu sửa chữa thành công!";
                return RedirectToAction("Index", "Home", new { area = "KhachHang" });
            }

            return View(phieuSuaChuaVM);
        }
    }
}
