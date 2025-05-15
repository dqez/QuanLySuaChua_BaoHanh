using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.KyThuatVien.Controllers
{
    [Area("KyThuatVien")]
    [Authorize(Roles = "KyThuatVien")]
    public class PhieuSuaChuaController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;
        private readonly UserManager<NguoiDung> _userManager;
        public PhieuSuaChuaController(BHSC_DbContext context, IDGenerator iDGenerator, UserManager<NguoiDung> userManager)
        {
            _context = context;
            _idGenerator = iDGenerator;
            _userManager = userManager;
        }

        // GET: KyThuatVien/PhieuSuaChua
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            var phieuSuaChuas = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id &&
                      (p.TrangThai == TrangThaiPhieu.DaPhanCong.ToString() || p.TrangThai == TrangThaiPhieu.ChoKiemTra.ToString()))
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .OrderByDescending(p => p.NgayGui)
                .ToListAsync();

            return View(phieuSuaChuas);
        }        
        // GET: KyThuatVien/PhieuSuaChua/DangXuLy
        public async Task<IActionResult> DangXuLy()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var phieuSuaChuas = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id && 
                      (p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString()))
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .OrderByDescending(p => p.NgayGui)
                .ToListAsync();

            return View(phieuSuaChuas);
        }        
        // GET: KyThuatVien/PhieuSuaChua/HoanThanh
        public async Task<IActionResult> HoanThanh()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var phieuSuaChuas = await _context.PhieuSuaChuas
                .Where(p => p.KyThuatId == user.Id && 
                     (p.TrangThai == TrangThaiPhieu.DaSuaXong.ToString() || 
                      p.TrangThai == TrangThaiPhieu.DaThanhToan.ToString() ||
                      p.TrangThai == TrangThaiPhieu.DangVanChuyen.ToString() ||
                      p.TrangThai == TrangThaiPhieu.HoanThanh.ToString()))
                .Include(p => p.KhachHang)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .OrderByDescending(p => p.NgayTra)
                .ToListAsync();

            return View(phieuSuaChuas);
        }

        // GET: KyThuatVien/PhieuSuaChua/ChiTiet/5
        public async Task<IActionResult> ChiTiet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuSuaChua = await _context.PhieuSuaChuas
                .Include(p => p.KhachHang)
                .Include(p => p.Phuong)
                    .ThenInclude(p => p.Quan)
                        .ThenInclude(q => q.ThanhPho)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.SanPham)
                .Include(p => p.ChiTietSuaChuas)
                    .ThenInclude(c => c.LinhKien)
                .FirstOrDefaultAsync(m => m.PhieuSuaChuaId == id);

            if (phieuSuaChua == null)
            {
                return NotFound();
            }

            ViewBag.LinhKienList = await _context.LinhKiens
                .Where(l => l.SoLuongTon > 0)
                .ToListAsync();

            return View(phieuSuaChua);
        }      

        // GET: KyThuatVien/PhieuSuaChua/BatDauXuLy/5
        public async Task<IActionResult> BatDauXuLy(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var phieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
            if (phieuSuaChua == null || phieuSuaChua.KyThuatId != user.Id)
            {
                return NotFound();
            }           
            phieuSuaChua.TrangThai = TrangThaiPhieu.DangSuaChua.ToString();
            _context.Update(phieuSuaChua);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã bắt đầu xử lý đơn sửa chữa.";
            return RedirectToAction(nameof(DangXuLy));
        }

        // POST: KyThuatVien/PhieuSuaChua/ThemLinhKien
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemLinhKien(string phieuSuaChuaId, string linhKienId, string sanPhamId, int soLuongLinhKien)
        {
            if (string.IsNullOrEmpty(phieuSuaChuaId) || string.IsNullOrEmpty(linhKienId) || string.IsNullOrEmpty(sanPhamId) || soLuongLinhKien <= 0)
            {
                return BadRequest();
            }

            var linhKien = await _context.LinhKiens.FindAsync(linhKienId);
            if (linhKien == null || linhKien.SoLuongTon < soLuongLinhKien)
            {
                TempData["Error"] = "Số lượng linh kiện không đủ.";
                return RedirectToAction(nameof(ChiTiet), new { id = phieuSuaChuaId });
            }

            var sanPham = await _context.SanPhams.FindAsync(sanPhamId);

            if (sanPham == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction(nameof(ChiTiet), new { id = phieuSuaChuaId });
            }

            var chiTietSuaChuaOlds = await _context.ChiTietSuaChuas
                .FirstOrDefaultAsync(c => c.PhieuSuaChuaId == phieuSuaChuaId && c.SanPhamId == sanPhamId && linhKienId == null);
           if(chiTietSuaChuaOlds != null)
           {
                chiTietSuaChuaOlds.LinhKienId = linhKienId;
                chiTietSuaChuaOlds.SoLuongLinhKien += soLuongLinhKien;
                chiTietSuaChuaOlds.LoaiDon = sanPham.NgayHetHanBh > DateOnly.FromDateTime(DateTime.Now) ? "SuaChua" : "BaoHanh";
                _context.ChiTietSuaChuas.Add(chiTietSuaChuaOlds);
            }
            else
            {
                chiTietSuaChuaOlds = new ChiTietSuaChua
                {
                    ChiTietId = await _idGenerator.GenerateIdAsync_Date("CTSC"),
                    PhieuSuaChuaId = phieuSuaChuaId,
                    SanPhamId = sanPhamId,
                    LinhKienId = linhKienId,
                    SoLuongLinhKien = soLuongLinhKien,
                    LoaiDon = sanPham.NgayHetHanBh > DateOnly.FromDateTime(DateTime.Now) ? "SuaChua" : "BaoHanh"
                };
            }               

            // Cập nhật số lượng tồn của linh kiện
            linhKien.SoLuongTon -= soLuongLinhKien;            

            _context.ChiTietSuaChuas.Add(chiTietSuaChuaOlds);
            _context.Update(linhKien);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm linh kiện thành công.";
            return RedirectToAction(nameof(ChiTiet), new { id = phieuSuaChuaId });
        }

        // POST: KyThuatVien/PhieuSuaChua/CapNhatDanhGia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatDanhGia(string chiTietId, string danhGiaKyThuat)
        {
            if (string.IsNullOrEmpty(chiTietId) || string.IsNullOrEmpty(danhGiaKyThuat))
            {
                return BadRequest();
            }

            var chiTietSuaChua = await _context.ChiTietSuaChuas
                .Include(c => c.PhieuSuaChua)
                .FirstOrDefaultAsync(c => c.ChiTietId == chiTietId);

            if (chiTietSuaChua == null)
            {
                return NotFound();
            }

            // Cập nhật đánh giá kỹ thuật
            chiTietSuaChua.DanhGiaKyThuat = danhGiaKyThuat;
            _context.Update(chiTietSuaChua);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật đánh giá thành công.";
            return RedirectToAction(nameof(ChiTiet), new { id = chiTietSuaChua.PhieuSuaChuaId });
        }

        // POST: KyThuatVien/PhieuSuaChua/CapNhatNgayHen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatNgayHen(string id, string ngayHen, string gioHen)
        {
            if (id == null || string.IsNullOrEmpty(ngayHen) || string.IsNullOrEmpty(gioHen))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin ngày giờ hẹn.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var phieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
            if (phieuSuaChua == null)
            {
                return NotFound();
            }

            // Kiểm tra nếu phiếu không ở trạng thái đã phân công
            if (phieuSuaChua.TrangThai != TrangThaiPhieu.DaPhanCong.ToString())
            {
                TempData["Error"] = "Phiếu sửa chữa không ở trạng thái chờ đặt lịch hẹn.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            // Xử lý chuyển đổi ngày giờ hẹn
            try 
            {
                DateTime ngay = DateTime.Parse(ngayHen);
                TimeSpan gio = TimeSpan.Parse(gioHen);
                
                var ngayGioHen = ngay.Date + gio;
                
                // Kiểm tra xem ngày hẹn có hợp lệ không (phải lớn hơn thời điểm hiện tại)
                if (ngayGioHen <= DateTime.Now)
                {
                    TempData["Error"] = "Ngày giờ hẹn phải lớn hơn thời điểm hiện tại.";
                    return RedirectToAction(nameof(ChiTiet), new { id });
                }

                // Cập nhật ngày hẹn
                phieuSuaChua.NgayHen = ngayGioHen;
                  // Cập nhật trạng thái sang "Đang sửa chữa" sau khi đặt ngày hẹn
                phieuSuaChua.TrangThai = TrangThaiPhieu.ChoKiemTra.ToString();
                
                _context.Update(phieuSuaChua);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã cập nhật ngày hẹn thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi xử lý ngày giờ: {ex.Message}";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }
        }

        // POST: KyThuatVien/PhieuSuaChua/HoanThanhDon/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanhDon(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phieuSuaChua = await _context.PhieuSuaChuas.FindAsync(id);
            if (phieuSuaChua == null)
            {
                return NotFound();
            }

            // Kiểm tra xem đã có đánh giá cho tất cả các chi tiết sửa chữa chưa
            var chiTietSuaChuas = await _context.ChiTietSuaChuas
                .Where(c => c.PhieuSuaChuaId == id)
                .ToListAsync();

            if (chiTietSuaChuas.Any(c => string.IsNullOrEmpty(c.DanhGiaKyThuat)))
            {
                TempData["Error"] = "Vui lòng cập nhật đánh giá kỹ thuật cho tất cả các linh kiện trước khi hoàn thành đơn.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }            
            // Cập nhật trạng thái phiếu sửa chữa thành đã sửa xong
            phieuSuaChua.TrangThai = TrangThaiPhieu.DaSuaXong.ToString();
            phieuSuaChua.NgayTra = DateTime.Now;

            // Tính tổng tiền
            decimal tongTien = 0;
            foreach (var chiTiet in chiTietSuaChuas)
            {
                var linhKien = await _context.LinhKiens.FindAsync(chiTiet.LinhKienId);
                if (linhKien != null)
                {
                    tongTien += linhKien.DonGia * chiTiet.SoLuongLinhKien;
                }
            }

            // Thêm phí vận chuyển (nếu có)
            if (phieuSuaChua.PhiVanChuyen.HasValue && phieuSuaChua.KhoangCach.HasValue)
            {
                tongTien += phieuSuaChua.PhiVanChuyen.Value * (decimal)phieuSuaChua.KhoangCach.Value;
            }

            phieuSuaChua.TongTien = tongTien;

            _context.Update(phieuSuaChua);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đơn sửa chữa đã hoàn thành.";
            return RedirectToAction(nameof(HoanThanh));
        }
    }
}
