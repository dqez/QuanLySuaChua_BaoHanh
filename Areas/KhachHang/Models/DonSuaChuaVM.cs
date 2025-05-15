using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Models
{
    public class DonSuaChuaVM
    {
        public string? PhieuSuaChuaId { get; set; } 
        public string? TrangThai { get; set; }
        public SanPham? SanPham { get; set; }
        public string? DiaChiNhanTraSanPham { get; set; }
        public string? MoTaKhachHang { get; set; }
        public string? DanhGiaKyThuat { get; set; }
        public string? TongTien { get; set; }
    }
}   
