using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Models
{
    public class PhieuNhapCreateViewModel
    {
        public PhieuNhap PhieuNhap { get; set; }
        public List<LinhKienInputModel> LinhKienInputs { get; set; } = new();
    }

}
