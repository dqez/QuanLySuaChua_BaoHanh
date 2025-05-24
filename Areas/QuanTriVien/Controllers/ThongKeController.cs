using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models;
using QuanLySuaChua_BaoHanh.Enums;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]    public class ThongKeController : Controller
    {
        private readonly BHSC_DbContext _context;
        private readonly IExportService _exportService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<NguoiDung> _userManager;

        public ThongKeController(BHSC_DbContext context, IExportService exportService, Microsoft.AspNetCore.Identity.UserManager<NguoiDung> userManager)
        {
            _context = context;
            _exportService = exportService;
            _userManager = userManager;
        }

        private void ValidateAndPrepareDateRange(ref DateTime? tuNgay, ref DateTime? denNgay, int defaultPastDays = 30)
        {
            if (!tuNgay.HasValue)
                tuNgay = DateTime.Now.AddDays(-defaultPastDays);
            if (!denNgay.HasValue)
                denNgay = DateTime.Now;

            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("DateRangeError", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.");
                // Đặt lại giá trị mặc định nếu có lỗi
                tuNgay = DateTime.Now.AddDays(-defaultPastDays);
                denNgay = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
        }

        // GET: QuanTriVien/ThongKe
        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);

            var viewModel = new ThongKeViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,

                // Thống kê tổng quan
                TongSoDonSuaChua = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                    .CountAsync(),

                TongDoanhThu = await _context.PhieuSuaChuas
                    .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                    .SumAsync(p => p.TongTien ?? 0),

                // Đơn hàng theo trạng thái
                SoDonChoXacNhan = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.ChoXacNhan.ToString())
                    .CountAsync(),

                SoDonDaXacNhan = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay &&
                           (p.TrangThai == TrangThaiPhieu.DaXacNhan.ToString()))
                    .CountAsync(),

                SoDonDaPhanCong = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DaPhanCong.ToString())
                    .CountAsync(),


                SoDonChoKiemTra = await _context.PhieuSuaChuas
                    .Where(p => p.NgayTra >= tuNgay && p.NgayTra <= denNgay && p.TrangThai == TrangThaiPhieu.ChoKiemTra.ToString())
                    .CountAsync(),

                SoDonDangSuaChua = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString())
                    .CountAsync(),
                SoDonDaSuaXong = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DaSuaXong.ToString())
                    .CountAsync(),
                SoDonDaThanhToan = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DaThanhToan.ToString())
                    .CountAsync(),
                SoDonDangVanChuyen = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DangVanChuyen.ToString())
                    .CountAsync(),
                SoDonDaHoanThanh = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                    .CountAsync(),
                SoDonDaHuy = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.TrangThai == TrangThaiPhieu.DaHuy.ToString())
                    .CountAsync(),

                // Thống kê phụ tùng/linh kiện
                SoLuongLinhKienSuDung = await _context.ChiTietSuaChuas
                    .Where(ct => ct.PhieuSuaChua.NgayGui >= tuNgay && ct.PhieuSuaChua.NgayGui <= denNgay && ct.LinhKienId != null)
                    .SumAsync(ct => ct.SoLuongLinhKien),

                // Top 10 kỹ thuật viên có số lượng đơn hàng nhiều nhất
                TopKyThuatVien = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && p.KyThuatId != null)
                    .GroupBy(p => new { p.KyThuatId, TenKyThuat = p.KyThuat.HoTen })
                    .Select(g => new TopKyThuatVienItem
                    {
                        KyThuatId = g.Key.KyThuatId,
                        TenKyThuat = g.Key.TenKyThuat,
                        SoLuongDon = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuongDon)
                    .Take(10)
                    .ToListAsync(),

                // Top 5 linh kiện sử dụng nhiều nhất
                TopLinhKien = await _context.ChiTietSuaChuas
                    .Where(ct => ct.PhieuSuaChua.NgayGui >= tuNgay && ct.PhieuSuaChua.NgayGui <= denNgay && ct.LinhKienId != null)
                    .GroupBy(ct => new { ct.LinhKienId, TenLinhKien = ct.LinhKien.TenLinhKien })
                    .Select(g => new TopLinhKienItem
                    {
                        LinhKienId = g.Key.LinhKienId,
                        TenLinhKien = g.Key.TenLinhKien,
                        SoLuongSuDung = g.Sum(ct => ct.SoLuongLinhKien)
                    })
                    .OrderByDescending(x => x.SoLuongSuDung)
                    .Take(5)
                    .ToListAsync(),

                // Dữ liệu cho biểu đồ
                DoanhThuTheoNgay = await _context.PhieuSuaChuas
                    .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                    .GroupBy(p => p.NgayThanhToan.Value.Date)
                    .Select(g => new DoanhThuNgayItem
                    {
                        Ngay = g.Key,
                        TongDoanhThu = g.Sum(p => p.TongTien ?? 0)
                    })
                    .OrderBy(x => x.Ngay)
                    .ToListAsync()
            };

            // Thêm thống kê theo khu vực
            viewModel.ThongKeThanhPho = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => new { p.Phuong.Quan.ThanhPhoId, TenThanhPho = p.Phuong.Quan.ThanhPho.TenThanhPho })
                .Select(g => new ThongKeThanhPhoItem
                {
                    ThanhPhoId = g.Key.ThanhPhoId,
                    TenThanhPho = g.Key.TenThanhPho,
                    SoLuongDon = g.Count(),
                    TongDoanhThu = g.Sum(p => p.TongTien ?? 0)
                })
                .OrderByDescending(x => x.SoLuongDon)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: QuanTriVien/ThongKe/DoanhThu
        public async Task<IActionResult> DoanhThu(DateTime? tuNgay, DateTime? denNgay, int? trang)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay, 90);
            int pageNumber = trang ?? 1;
            int pageSize = 10;

            // Đảm bảo ngày kết thúc là cuối ngày để bao gồm cả dữ liệu của ngày đó
            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            // Kiểm tra tính hợp lệ của khoảng thời gian
            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                // Đặt lại giá trị mặc định
                tuNgay = DateTime.Now.AddDays(-90);
                denNgay = DateTime.Now;
            }

            var doanhThuTheoThangQuery = _context.PhieuSuaChuas
                .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                .GroupBy(p => new { Thang = p.NgayThanhToan.Value.Month, Nam = p.NgayThanhToan.Value.Year })
                .Select(g => new DoanhThuThangItem
                {
                    Thang = g.Key.Thang,
                    Nam = g.Key.Nam,
                    TongDoanhThu = g.Sum(p => p.TongTien ?? 0),
                    SoLuongDon = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .AsQueryable();

            var viewModel = new DoanhThuViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,
                TrangHienTai = pageNumber,

                TongDoanhThu = await _context.PhieuSuaChuas
                    .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                    .SumAsync(p => p.TongTien ?? 0),

                SoLuongDonHoanThanh = await _context.PhieuSuaChuas
                    .Where(p => p.NgayTra >= tuNgay && p.NgayTra <= denNgay && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                    .CountAsync(),

                DoanhThuTheoThang = await Models.PaginatedList<DoanhThuThangItem>.CreateAsync(
                    doanhThuTheoThangQuery, pageNumber, pageSize),

                DoanhThuTheoPhuongThucThanhToan = await _context.PhieuSuaChuas
                    .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue && !string.IsNullOrEmpty(p.PhuongThucThanhToan))
                    .GroupBy(p => p.PhuongThucThanhToan)
                    .Select(g => new DoanhThuTheoPhuongThucItem
                    {
                        PhuongThucThanhToan = g.Key,
                        TongDoanhThu = g.Sum(p => p.TongTien ?? 0),
                        SoLuongGiaoDich = g.Count()
                    })
                    .OrderByDescending(x => x.TongDoanhThu)
                    .ToListAsync()
            };

            return View(viewModel);
        }        // GET: QuanTriVien/ThongKe/DonSuaChua
        public async Task<IActionResult> DonSuaChua(DateTime? tuNgay, DateTime? denNgay, int? trang)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);
            int pageNumber = trang ?? 1;
            int pageSize = 10;

            // Đảm bảo ngày kết thúc là cuối ngày để bao gồm cả dữ liệu của ngày đó
            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            // Kiểm tra tính hợp lệ của khoảng thời gian
            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                // Đặt lại giá trị mặc định
                tuNgay = DateTime.Now.AddDays(-30);
                denNgay = DateTime.Now;
            }

            var donTheoNgayQuery = _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => p.NgayGui.Date)
                .Select(g => new DonTheoNgayItem
                {
                    Ngay = g.Key,
                    SoLuongDon = g.Count()
                })
                .OrderBy(x => x.Ngay)
                .AsQueryable();

            var viewModel = new DonSuaChuaViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,
                TrangHienTai = pageNumber,

                TongSoDon = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                    .CountAsync(),

                ThongKeTrangThai = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                    .GroupBy(p => p.TrangThai)
                    .Select(g => new ThongKeTrangThaiItem
                    {
                        TrangThai = g.Key,
                        SoLuong = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuong)
                    .ToListAsync(),                ThoiGianXuLyTrungBinh = await _context.PhieuSuaChuas
                    .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && 
                           p.NgayTra.HasValue && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                    .Select(p => new { NgayGui = p.NgayGui, NgayTra = p.NgayTra.Value })
                    .ToListAsync()
                    .ContinueWith(t => t.Result
                        .Select(p => (p.NgayTra - p.NgayGui).TotalDays)
                        .DefaultIfEmpty(0)
                        .Average()),

                DonTheoNgay = await Models.PaginatedList<DonTheoNgayItem>.CreateAsync(
                    donTheoNgayQuery, pageNumber, pageSize)
            };

            return View(viewModel);
        }        // GET: QuanTriVien/ThongKe/KhoLinhKien
        public async Task<IActionResult> KhoLinhKien(string danhMucId, int? trang)
        {
            int pageNumber = trang ?? 1;
            int pageSize = 10;

            // Lọc theo danh mục nếu có
            var linhKienQuery = _context.LinhKiens
                .Include(lk => lk.DanhMuc)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(danhMucId))
            {
                linhKienQuery = linhKienQuery.Where(lk => lk.DanhMucId == danhMucId);
            }

            // Thống kê linh kiện tồn kho với phân trang
            var thongKeTonKhoQuery = linhKienQuery
                .Select(lk => new ThongKeTonKhoItem
                {
                    LinhKienId = lk.LinhKienId,
                    TenLinhKien = lk.TenLinhKien,
                    DanhMuc = lk.DanhMuc.TenDanhMuc,
                    SoLuongTon = lk.SoLuongTon,
                    DonGia = lk.DonGia,
                    GiaTriTon = lk.SoLuongTon * lk.DonGia
                })
                .OrderByDescending(x => x.GiaTriTon);

            var viewModel = new KhoLinhKienViewModel
            {
                TongSoLinhKien = await _context.LinhKiens.CountAsync(),
                TongGiaTriTonKho = await _context.LinhKiens.SumAsync(lk => lk.SoLuongTon * lk.DonGia),
                DanhMucId = danhMucId,
                TrangHienTai = pageNumber,

                DanhSachDanhMuc = await _context.DanhMucs
                    .OrderBy(dm => dm.TenDanhMuc)
                    .Select(dm => new SelectListItem
                    {
                        Value = dm.DanhMucId,
                        Text = dm.TenDanhMuc
                    })
                    .ToListAsync(),

                ThongKeTonKho = await NhanVienKho.Models.PaginatedList<ThongKeTonKhoItem>.CreateAsync(
                    thongKeTonKhoQuery, pageNumber, pageSize),

                // Thống kê theo danh mục
                ThongKeTheoDanhMuc = await _context.LinhKiens
                    .GroupBy(lk => new { lk.DanhMucId, TenDanhMuc = lk.DanhMuc.TenDanhMuc })
                    .Select(g => new ThongKeTheoDanhMucItem
                    {
                        DanhMucId = g.Key.DanhMucId,
                        TenDanhMuc = g.Key.TenDanhMuc,
                        SoLuongLinhKien = g.Count(),
                        TongSoLuongTon = g.Sum(lk => lk.SoLuongTon),
                        TongGiaTri = g.Sum(lk => lk.SoLuongTon * lk.DonGia)
                    })
                    .OrderByDescending(x => x.TongGiaTri)
                    .ToListAsync()
            };

            return View(viewModel);
        }        // GET: QuanTriVien/ThongKe/KyThuatVien
        public async Task<IActionResult> KyThuatVien(DateTime? tuNgay, DateTime? denNgay, int? trang)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);
            int pageNumber = trang ?? 1;
            int pageSize = 10;

            // Đảm bảo ngày kết thúc là cuối ngày để bao gồm cả dữ liệu của ngày đó
            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            // Kiểm tra tính hợp lệ của khoảng thời gian
            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                // Đặt lại giá trị mặc định
                tuNgay = DateTime.Now.AddDays(-30);
                denNgay = DateTime.Now;
            }

            // Lấy danh sách kỹ thuật viên
            var kyThuatViens = await _userManager.GetUsersInRoleAsync("KyThuatVien");
            var kyThuatVienIds = kyThuatViens.Select(u => u.Id).ToList();            // Lấy dữ liệu cơ bản
            var dataQuery = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && 
                       kyThuatVienIds.Contains(p.KyThuatId))
                .Select(p => new {
                    p.KyThuatId,
                    TenKyThuat = p.KyThuat.HoTen,
                    p.TrangThai,
                    p.NgayGui,
                    p.NgayTra
                })
                .ToListAsync();            // Xử lý dữ liệu trên client
            var hieuSuatKyThuatVien = dataQuery
                .GroupBy(p => new { p.KyThuatId, p.TenKyThuat })
                .Select(g => new HieuSuatKyThuatVienItem
                {
                    KyThuatId = g.Key.KyThuatId,
                    TenKyThuat = g.Key.TenKyThuat,
                    TongSoDon = g.Count(),
                    SoDonHoanThanh = g.Count(p => p.TrangThai == TrangThaiPhieu.HoanThanh.ToString()),
                    SoDonDangXuLy = g.Count(p => p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString()),
                    ThoiGianTrungBinh = g
                        .Where(p => p.NgayTra.HasValue && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                        .Select(p => (p.NgayTra.Value - p.NgayGui).TotalDays)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(x => x.TongSoDon)
                .ToList();

            // Thực hiện phân trang trên danh sách đã được tải về
            var totalItems = hieuSuatKyThuatVien.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = hieuSuatKyThuatVien
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new KyThuatVienViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,
                TrangHienTai = pageNumber,

                // Tạo đối tượng phân trang thủ công
                HieuSuatKyThuatVien = new NhanVienKho.Models.PaginatedList<HieuSuatKyThuatVienItem>(
                    items, totalItems, pageNumber, pageSize)
            };

            return View(viewModel);
        }

        // GET: QuanTriVien/ThongKe/KhachHang
        public async Task<IActionResult> KhachHang(DateTime? tuNgay, DateTime? denNgay, int? trang)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);
            int pageNumber = trang ?? 1;
            int pageSize = 10;

            // Đảm bảo ngày kết thúc là cuối ngày để bao gồm cả dữ liệu của ngày đó
            denNgay = denNgay.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            // Kiểm tra tính hợp lệ của khoảng thời gian
            if (tuNgay > denNgay)
            {
                ModelState.AddModelError("", "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");
                // Đặt lại giá trị mặc định
                tuNgay = DateTime.Now.AddDays(-30);
                denNgay = DateTime.Now;
            }

            // Thống kê khách hàng với phân trang
            var thongKeKhachHangQuery = _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => new { p.KhachHangId, TenKhachHang = p.KhachHang.HoTen })
                .Select(g => new ThongKeKhachHangItem
                {
                    KhachHangId = g.Key.KhachHangId,
                    TenKhachHang = g.Key.TenKhachHang,
                    SoDonHang = g.Count(),
                    TongChiTieu = g.Sum(p => p.TongTien ?? 0)
                })
                .OrderByDescending(x => x.TongChiTieu)
                .AsQueryable();

            var viewModel = new KhachHangViewModel
            {
                TuNgay = tuNgay.Value,
                DenNgay = denNgay.Value,
                TrangHienTai = pageNumber,

                TongSoKhachHang = await _context.Users
                    .Where(u => u.VaiTro == "KhachHang")
                    .CountAsync(),


                ThongKeKhachHang = await NhanVienKho.Models.PaginatedList<ThongKeKhachHangItem>.CreateAsync(
                    thongKeKhachHangQuery, pageNumber, pageSize),

                ThongKeTheoKhuVuc = await _context.Users
                    .Where(u => u.VaiTro == "KhachHang")
                    .GroupBy(u => new { u.Phuong.Quan.ThanhPhoId, TenThanhPho = u.Phuong.Quan.ThanhPho.TenThanhPho })
                    .Select(g => new ThongKeKhachHangTheoKhuVucItem
                    {
                        ThanhPhoId = g.Key.ThanhPhoId,
                        TenThanhPho = g.Key.TenThanhPho,
                        SoLuongKhachHang = g.Count()
                    })
                    .OrderByDescending(x => x.SoLuongKhachHang)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: QuanTriVien/ThongKe/ExportDoanhThu
        public async Task<IActionResult> ExportDoanhThu(DateTime? tuNgay, DateTime? denNgay, string format)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay, 90);

            var doanhThuTheoThang = await _context.PhieuSuaChuas
                .Where(p => p.NgayThanhToan >= tuNgay && p.NgayThanhToan <= denNgay && p.TongTien.HasValue)
                .GroupBy(p => new { Thang = p.NgayThanhToan.Value.Month, Nam = p.NgayThanhToan.Value.Year })
                .Select(g => new DoanhThuThangItem
                {
                    Thang = g.Key.Thang,
                    Nam = g.Key.Nam,
                    TongDoanhThu = g.Sum(p => p.TongTien ?? 0),
                    SoLuongDon = g.Count()
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            if (format.ToLower() == "excel")
            {
                var columnMappings = new Dictionary<string, string>
                {
                    { "ThangNam", "Tháng/Năm" },
                    { "SoLuongDon", "Số lượng đơn" },
                    { "TongDoanhThu", "Tổng doanh thu (VNĐ)" }
                };

                var excelData = _exportService.ExportToExcel(
                    doanhThuTheoThang,
                    "DoanhThu",
                    columnMappings,
                    $"Báo cáo doanh thu từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    excelData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Doanh_Thu_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            else // PDF
            {
                var htmlContent = new StringBuilder();
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Tháng/Năm</th><th>Số lượng đơn</th><th>Tổng doanh thu (VNĐ)</th></tr>");

                foreach (var item in doanhThuTheoThang)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.ThangNam}</td>");
                    htmlContent.Append($"<td>{item.SoLuongDon}</td>");
                    htmlContent.Append($"<td>{item.TongDoanhThu:N0}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                var pdfData = _exportService.ExportToPdf(
                    htmlContent.ToString(),
                    $"Báo cáo doanh thu từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    pdfData,
                    "application/pdf",
                    $"Doanh_Thu_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        // GET: QuanTriVien/ThongKe/ExportDonSuaChua
        public async Task<IActionResult> ExportDonSuaChua(DateTime? tuNgay, DateTime? denNgay, string format)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);

            var thongKeTrangThai = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => p.TrangThai)
                .Select(g => new ThongKeTrangThaiItem
                {
                    TrangThai = g.Key, // Giữ nguyên g.Key vì đây là group key, sẽ được chuyển đổi ở view nếu cần
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            var donTheoNgay = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => p.NgayGui.Date)
                .Select(g => new DonTheoNgayItem
                {
                    Ngay = g.Key,
                    SoLuongDon = g.Count()
                })
                .OrderBy(x => x.Ngay)
                .ToListAsync();

            if (format.ToLower() == "excel")
            {
                using (var package = new ExcelPackage())
                {
                    // Tạo sheet cho thống kê theo trạng thái
                    var columnMappingsTrangThai = new Dictionary<string, string>
                    {
                        { "TrangThai", "Trạng thái" },
                        { "SoLuong", "Số lượng" }
                    };

                    var excelDataTrangThai = _exportService.ExportToExcel(
                        thongKeTrangThai,
                        "ThongKeTrangThai",
                        columnMappingsTrangThai,
                        $"Thống kê đơn sửa chữa theo trạng thái từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                    // Tạo sheet cho thống kê theo ngày
                    var columnMappingsTheoNgay = new Dictionary<string, string>
                    {
                        { "Ngay", "Ngày" },
                        { "SoLuongDon", "Số lượng đơn" }
                    };

                    var excelDataTheoNgay = _exportService.ExportToExcel(
                        donTheoNgay,
                        "ThongKeTheoNgay",
                        columnMappingsTheoNgay,
                        $"Thống kê đơn sửa chữa theo ngày từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                    return File(
                        excelDataTrangThai, // Sử dụng sheet trạng thái làm chính
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Don_Sua_Chua_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
            else // PDF
            {
                var htmlContent = new StringBuilder();
                
                // Thêm bảng thống kê trạng thái
                htmlContent.Append("<h2>Thống kê theo trạng thái</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Trạng thái</th><th>Số lượng</th></tr>");

                foreach (var item in thongKeTrangThai)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TrangThai}</td>");
                    htmlContent.Append($"<td>{item.SoLuong}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                // Thêm bảng thống kê theo ngày
                htmlContent.Append("<h2>Thống kê theo ngày</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Ngày</th><th>Số lượng đơn</th></tr>");

                foreach (var item in donTheoNgay)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.Ngay:dd/MM/yyyy}</td>");
                    htmlContent.Append($"<td>{item.SoLuongDon}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                var pdfData = _exportService.ExportToPdf(
                    htmlContent.ToString(),
                    $"Báo cáo đơn sửa chữa từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    pdfData,
                    "application/pdf",
                    $"Don_Sua_Chua_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        // GET: QuanTriVien/ThongKe/ExportKhoLinhKien
        public async Task<IActionResult> ExportKhoLinhKien(string danhMucId, string format)
        {
            // Lọc theo danh mục nếu có
            var linhKienQuery = _context.LinhKiens.Include(lk => lk.DanhMuc).AsQueryable();
            
            if (!string.IsNullOrEmpty(danhMucId))
            {
                linhKienQuery = linhKienQuery.Where(lk => lk.DanhMucId == danhMucId);
            }

            // Thống kê linh kiện tồn kho
            var thongKeTonKho = await linhKienQuery
                .Select(lk => new ThongKeTonKhoItem
                {
                    LinhKienId = lk.LinhKienId,
                    TenLinhKien = lk.TenLinhKien,
                    DanhMuc = lk.DanhMuc.TenDanhMuc,
                    SoLuongTon = lk.SoLuongTon,
                    DonGia = lk.DonGia,
                    GiaTriTon = lk.SoLuongTon * lk.DonGia
                })
                .OrderByDescending(x => x.GiaTriTon)
                .ToListAsync();

            // Thống kê theo danh mục
            var thongKeTheoDanhMuc = await _context.LinhKiens
                .GroupBy(lk => new { lk.DanhMucId, TenDanhMuc = lk.DanhMuc.TenDanhMuc })
                .Select(g => new ThongKeTheoDanhMucItem
                {
                    DanhMucId = g.Key.DanhMucId,
                    TenDanhMuc = g.Key.TenDanhMuc,
                    SoLuongLinhKien = g.Count(),
                    TongSoLuongTon = g.Sum(lk => lk.SoLuongTon),
                    TongGiaTri = g.Sum(lk => lk.SoLuongTon * lk.DonGia)
                })
                .OrderByDescending(x => x.TongGiaTri)
                .ToListAsync();

            string titleFilter = string.IsNullOrEmpty(danhMucId) 
                ? "tất cả danh mục" 
                : $"danh mục {thongKeTheoDanhMuc.FirstOrDefault(d => d.DanhMucId == danhMucId)?.TenDanhMuc}";

            if (format.ToLower() == "excel")
            {
                using (var package = new ExcelPackage())
                {
                    // Tạo sheet cho thống kê tồn kho
                    var columnMappingsTonKho = new Dictionary<string, string>
                    {
                        { "TenLinhKien", "Tên linh kiện" },
                        { "DanhMuc", "Danh mục" },
                        { "SoLuongTon", "Số lượng tồn" },
                        { "DonGia", "Đơn giá (VNĐ)" },
                        { "GiaTriTon", "Giá trị tồn (VNĐ)" }
                    };

                    var excelDataTonKho = _exportService.ExportToExcel(
                        thongKeTonKho,
                        "ThongKeTonKho",
                        columnMappingsTonKho,
                        $"Thống kê tồn kho - {titleFilter}");

                    // Tạo sheet cho thống kê theo danh mục
                    var columnMappingsTheoDanhMuc = new Dictionary<string, string>
                    {
                        { "TenDanhMuc", "Tên danh mục" },
                        { "SoLuongLinhKien", "Số loại linh kiện" },
                        { "TongSoLuongTon", "Tổng số lượng tồn" },
                        { "TongGiaTri", "Tổng giá trị (VNĐ)" }
                    };

                    var excelDataTheoDanhMuc = _exportService.ExportToExcel(
                        thongKeTheoDanhMuc,
                        "ThongKeTheoDanhMuc",
                        columnMappingsTheoDanhMuc,
                        "Thống kê theo danh mục");

                    return File(
                        excelDataTonKho,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Kho_Linh_Kien_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
            else // PDF
            {
                var htmlContent = new StringBuilder();
                
                // Thêm bảng thống kê tồn kho
                htmlContent.Append("<h2>Thống kê tồn kho</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Tên linh kiện</th><th>Danh mục</th><th>Số lượng tồn</th><th>Đơn giá (VNĐ)</th><th>Giá trị tồn (VNĐ)</th></tr>");

                foreach (var item in thongKeTonKho)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TenLinhKien}</td>");
                    htmlContent.Append($"<td>{item.DanhMuc}</td>");
                    htmlContent.Append($"<td>{item.SoLuongTon}</td>");
                    htmlContent.Append($"<td>{item.DonGia:N0}</td>");
                    htmlContent.Append($"<td>{item.GiaTriTon:N0}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                // Thêm bảng thống kê theo danh mục
                htmlContent.Append("<h2>Thống kê theo danh mục</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Tên danh mục</th><th>Số loại linh kiện</th><th>Tổng số lượng tồn</th><th>Tổng giá trị (VNĐ)</th></tr>");

                foreach (var item in thongKeTheoDanhMuc)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TenDanhMuc}</td>");
                    htmlContent.Append($"<td>{item.SoLuongLinhKien}</td>");
                    htmlContent.Append($"<td>{item.TongSoLuongTon}</td>");
                    htmlContent.Append($"<td>{item.TongGiaTri:N0}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                var pdfData = _exportService.ExportToPdf(
                    htmlContent.ToString(),
                    $"Báo cáo kho linh kiện - {titleFilter}");

                return File(
                    pdfData,
                    "application/pdf",
                    $"Kho_Linh_Kien_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        // GET: QuanTriVien/ThongKe/ExportKyThuatVien
        public async Task<IActionResult> ExportKyThuatVien(DateTime? tuNgay, DateTime? denNgay, string format)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);

            // Lấy danh sách kỹ thuật viên
            var kyThuatViens = await _userManager.GetUsersInRoleAsync("KyThuatVien");
            var kyThuatVienIds = kyThuatViens.Select(u => u.Id).ToList();

            // Thống kê hiệu suất
            var hieuSuatKyThuatVien = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay && 
                       kyThuatVienIds.Contains(p.KyThuatId))
                .GroupBy(p => new { p.KyThuatId, TenKyThuat = p.KyThuat.HoTen })
                .Select(g => new HieuSuatKyThuatVienItem
                {
                    KyThuatId = g.Key.KyThuatId,
                    TenKyThuat = g.Key.TenKyThuat,
                    TongSoDon = g.Count(),
                    SoDonHoanThanh = g.Count(p => p.TrangThai == TrangThaiPhieu.HoanThanh.ToString()),
                    SoDonDangXuLy = g.Count(p =>  p.TrangThai == TrangThaiPhieu.DangSuaChua.ToString()),
                    ThoiGianTrungBinh = g.Where(p => p.NgayTra.HasValue && p.TrangThai == TrangThaiPhieu.HoanThanh.ToString())
                        .Select(p => (double)(p.NgayTra.Value - p.NgayGui).TotalDays)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(x => x.TongSoDon)
                .ToListAsync();

            if (format.ToLower() == "excel")
            {
                var columnMappings = new Dictionary<string, string>
                {
                    { "TenKyThuat", "Tên kỹ thuật viên" },
                    { "TongSoDon", "Tổng số đơn" },
                    { "SoDonHoanThanh", "Đơn hoàn thành" },
                    { "SoDonDangXuLy", "Đơn đang xử lý" },
                    { "ThoiGianTrungBinh", "Thời gian xử lý TB (ngày)" },
                    { "TyLeHoanThanh", "Tỷ lệ hoàn thành (%)" }
                };

                var excelData = _exportService.ExportToExcel(
                    hieuSuatKyThuatVien,
                    "HieuSuatKyThuatVien",
                    columnMappings,
                    $"Báo cáo hiệu suất kỹ thuật viên từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    excelData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Ky_Thuat_Vien_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            else // PDF
            {
                var htmlContent = new StringBuilder();
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Tên kỹ thuật viên</th><th>Tổng số đơn</th><th>Đơn hoàn thành</th><th>Đơn đang xử lý</th><th>Thời gian xử lý TB (ngày)</th><th>Tỷ lệ hoàn thành (%)</th></tr>");

                foreach (var item in hieuSuatKyThuatVien)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TenKyThuat}</td>");
                    htmlContent.Append($"<td>{item.TongSoDon}</td>");
                    htmlContent.Append($"<td>{item.SoDonHoanThanh}</td>");
                    htmlContent.Append($"<td>{item.SoDonDangXuLy}</td>");
                    htmlContent.Append($"<td>{item.ThoiGianTrungBinh:F1}</td>");
                    htmlContent.Append($"<td>{item.TyLeHoanThanh:F1}%</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                var pdfData = _exportService.ExportToPdf(
                    htmlContent.ToString(),
                    $"Báo cáo hiệu suất kỹ thuật viên từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    pdfData,
                    "application/pdf",
                    $"Ky_Thuat_Vien_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        // GET: QuanTriVien/ThongKe/ExportKhachHang
        public async Task<IActionResult> ExportKhachHang(DateTime? tuNgay, DateTime? denNgay, string format)
        {
            ValidateAndPrepareDateRange(ref tuNgay, ref denNgay);

            // Thống kê khách hàng
            var thongKeKhachHang = await _context.PhieuSuaChuas
                .Where(p => p.NgayGui >= tuNgay && p.NgayGui <= denNgay)
                .GroupBy(p => new { p.KhachHangId, TenKhachHang = p.KhachHang.HoTen })
                .Select(g => new ThongKeKhachHangItem
                {
                    KhachHangId = g.Key.KhachHangId,
                    TenKhachHang = g.Key.TenKhachHang,
                    SoDonHang = g.Count(),
                    TongChiTieu = g.Sum(p => p.TongTien ?? 0)
                })
                .OrderByDescending(x => x.TongChiTieu)
                .Take(50) // Giới hạn để không quá lớn
                .ToListAsync();

            // Thống kê theo khu vực
            var thongKeTheoKhuVuc = await _context.Users
                .Where(u => u.VaiTro == "KhachHang")
                .GroupBy(u => new { u.Phuong.Quan.ThanhPhoId, TenThanhPho = u.Phuong.Quan.ThanhPho.TenThanhPho })
                .Select(g => new ThongKeKhachHangTheoKhuVucItem
                {
                    ThanhPhoId = g.Key.ThanhPhoId,
                    TenThanhPho = g.Key.TenThanhPho,
                    SoLuongKhachHang = g.Count()
                })
                .OrderByDescending(x => x.SoLuongKhachHang)
                .ToListAsync();

            if (format.ToLower() == "excel")
            {
                // Tạo sheet cho thống kê khách hàng
                var columnMappingsKhachHang = new Dictionary<string, string>
                {
                    { "TenKhachHang", "Tên khách hàng" },
                    { "SoDonHang", "Số đơn hàng" },
                    { "TongChiTieu", "Tổng chi tiêu (VNĐ)" }
                };

                var excelDataKhachHang = _exportService.ExportToExcel(
                    thongKeKhachHang,
                    "ThongKeKhachHang",
                    columnMappingsKhachHang,
                    $"Báo cáo khách hàng từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    excelDataKhachHang,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Khach_Hang_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            else // PDF
            {
                var htmlContent = new StringBuilder();
                
                // Thêm bảng thống kê khách hàng
                htmlContent.Append("<h2>Thống kê khách hàng</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Tên khách hàng</th><th>Số đơn hàng</th><th>Tổng chi tiêu (VNĐ)</th></tr>");

                foreach (var item in thongKeKhachHang)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TenKhachHang}</td>");
                    htmlContent.Append($"<td>{item.SoDonHang}</td>");
                    htmlContent.Append($"<td>{item.TongChiTieu:N0}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                // Thêm bảng thống kê theo khu vực
                htmlContent.Append("<h2>Thống kê theo khu vực</h2>");
                htmlContent.Append("<table>");
                htmlContent.Append("<tr><th>Thành phố</th><th>Số lượng khách hàng</th></tr>");

                foreach (var item in thongKeTheoKhuVuc)
                {
                    htmlContent.Append("<tr>");
                    htmlContent.Append($"<td>{item.TenThanhPho}</td>");
                    htmlContent.Append($"<td>{item.SoLuongKhachHang}</td>");
                    htmlContent.Append("</tr>");
                }
                htmlContent.Append("</table>");

                var pdfData = _exportService.ExportToPdf(
                    htmlContent.ToString(),
                    $"Báo cáo khách hàng từ {tuNgay.Value:dd/MM/yyyy} đến {denNgay.Value:dd/MM/yyyy}");

                return File(
                    pdfData,
                    "application/pdf",
                    $"Khach_Hang_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }
    }
}
