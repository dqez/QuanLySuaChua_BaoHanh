using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }        [Display(Name = "Thành phố/Tỉnh")]
        public string ThanhPhoId { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string QuanId { get; set; }

        [Required(ErrorMessage = "Phường/Xã là bắt buộc")]
        [Display(Name = "Phường/Xã")]
        public string PhuongId { get; set; }

        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; }

        // Thông tin hiển thị
        [Display(Name = "Phường/Xã")]
        public string TenPhuong { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string TenQuan { get; set; }

        [Display(Name = "Thành phố/Tỉnh")]
        public string TenThanhPho { get; set; }
    }
}
