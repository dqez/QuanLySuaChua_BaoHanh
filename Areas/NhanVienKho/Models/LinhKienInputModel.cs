namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Models
{
    public class LinhKienInputModel
    {
        public string LinhKienId { get; set; }
        public string TenLinhKien { get; set; }
        public decimal? DonGia { get; set; }
        public bool IsSelected { get; set; }
        public int SoLuong { get; set; }
    }
}
