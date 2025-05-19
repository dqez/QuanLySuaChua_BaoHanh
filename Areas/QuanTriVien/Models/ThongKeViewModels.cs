using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models
{
    public class ThongKeViewModel
    {
        // Phạm vi thời gian thống kê
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        // Thống kê tổng quan
        public int TongSoDonSuaChua { get; set; }
        public decimal TongDoanhThu { get; set; }

        // Thống kê đơn hàng theo trạng thái
        public int SoDonChoXacNhan { get; set; }
        public int SoDonDangXuLy { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonHuy { get; set; }

        // Thống kê linh kiện
        public int SoLuongLinhKienSuDung { get; set; }

        // Dữ liệu chi tiết
        public List<TopKyThuatVienItem> TopKyThuatVien { get; set; }
        public List<TopLinhKienItem> TopLinhKien { get; set; }
        public List<DoanhThuNgayItem> DoanhThuTheoNgay { get; set; }
        public List<ThongKeThanhPhoItem> ThongKeThanhPho { get; set; }
    }    public class DoanhThuViewModel
    {
        // Phạm vi thời gian thống kê
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        // Thống kê chung
        public decimal TongDoanhThu { get; set; }
        public int SoLuongDonHoanThanh { get; set; }

        // Thống kê chi tiết
        public PaginatedList<DoanhThuThangItem> DoanhThuTheoThang { get; set; }
        public List<DoanhThuTheoPhuongThucItem> DoanhThuTheoPhuongThucThanhToan { get; set; }
        
        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
    }    public class DonSuaChuaViewModel
    {
        // Phạm vi thời gian thống kê
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        // Thống kê chung
        public int TongSoDon { get; set; }
        public double ThoiGianXuLyTrungBinh { get; set; }

        // Thống kê chi tiết
        public List<ThongKeTrangThaiItem> ThongKeTrangThai { get; set; }
        public PaginatedList<DonTheoNgayItem> DonTheoNgay { get; set; }
        
        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
    }

    public class KhoLinhKienViewModel
    {
        // Thống kê chung
        public int TongSoLinhKien { get; set; }
        public decimal TongGiaTriTonKho { get; set; }
        
        // Lọc
        public string DanhMucId { get; set; }
        public List<SelectListItem> DanhSachDanhMuc { get; set; }

        // Thống kê chi tiết
        public NhanVienKho.Models.PaginatedList<ThongKeTonKhoItem> ThongKeTonKho { get; set; }
        public List<ThongKeTheoDanhMucItem> ThongKeTheoDanhMuc { get; set; }
        
        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
    }

    public class KyThuatVienViewModel
    {
        // Phạm vi thời gian thống kê
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        // Thống kê hiệu suất
        public NhanVienKho.Models.PaginatedList<HieuSuatKyThuatVienItem> HieuSuatKyThuatVien { get; set; }
        
        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
    }

    public class KhachHangViewModel
    {
        // Phạm vi thời gian thống kê
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        // Thống kê chung
        public int TongSoKhachHang { get; set; }
        public int KhachHangMoi { get; set; }

        // Thống kê chi tiết
        public NhanVienKho.Models.PaginatedList<ThongKeKhachHangItem> ThongKeKhachHang { get; set; }
        public List<ThongKeKhachHangTheoKhuVucItem> ThongKeTheoKhuVuc { get; set; }
        
        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
    }

    // Classes hỗ trợ
    public class TopKyThuatVienItem
    {
        public string KyThuatId { get; set; }
        public string TenKyThuat { get; set; }
        public int SoLuongDon { get; set; }
    }

    public class TopLinhKienItem
    {
        public string LinhKienId { get; set; }
        public string TenLinhKien { get; set; }
        public int SoLuongSuDung { get; set; }
    }

    public class DoanhThuNgayItem
    {
        public DateTime Ngay { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class ThongKeThanhPhoItem
    {
        public string ThanhPhoId { get; set; }
        public string TenThanhPho { get; set; }
        public int SoLuongDon { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class DoanhThuThangItem
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoLuongDon { get; set; }
        public string ThangNam => $"{Thang}/{Nam}";
    }

    public class DoanhThuTheoPhuongThucItem
    {
        public string PhuongThucThanhToan { get; set; }
        public decimal TongDoanhThu { get; set; }
        public int SoLuongGiaoDich { get; set; }
    }

    public class ThongKeTrangThaiItem
    {
        public string TrangThai { get; set; }
        public int SoLuong { get; set; }
    }

    public class DonTheoNgayItem
    {
        public DateTime Ngay { get; set; }
        public int SoLuongDon { get; set; }
    }

    public class ThongKeTonKhoItem
    {
        public string LinhKienId { get; set; }
        public string TenLinhKien { get; set; }
        public string DanhMuc { get; set; }
        public int SoLuongTon { get; set; }
        public decimal DonGia { get; set; }
        public decimal GiaTriTon { get; set; }
    }

    public class ThongKeTheoDanhMucItem
    {
        public string DanhMucId { get; set; }
        public string TenDanhMuc { get; set; }
        public int SoLuongLinhKien { get; set; }
        public int TongSoLuongTon { get; set; }
        public decimal TongGiaTri { get; set; }
    }

    public class HieuSuatKyThuatVienItem
    {
        public string KyThuatId { get; set; }
        public string TenKyThuat { get; set; }
        public int TongSoDon { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonDangXuLy { get; set; }
        public double ThoiGianTrungBinh { get; set; }
        
        public double TyLeHoanThanh => TongSoDon > 0 ? (double)SoDonHoanThanh / TongSoDon * 100 : 0;
    }

    public class ThongKeKhachHangItem
    {
        public string KhachHangId { get; set; }
        public string TenKhachHang { get; set; }
        public int SoDonHang { get; set; }
        public decimal TongChiTieu { get; set; }
    }

    public class ThongKeKhachHangTheoKhuVucItem
    {
        public string ThanhPhoId { get; set; }
        public string TenThanhPho { get; set; }
        public int SoLuongKhachHang { get; set; }
    }
}
