using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Models
{
    public class PhieuSuaChuaVM
    {
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một sản phẩm cần sửa")]
        [DisplayName("Sản phẩm cần sửa")]
        public List<string> SanPhamIds { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng chọn phường/xã")]
        public string PhuongId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận/trả sản phẩm")]
        [Display(Name = "Địa chỉ nhận/trả")]
        public string DiaChiNhanTraSanPham { get; set; }

        public string? MoTaKhachHang { get; set; }
    }
}
