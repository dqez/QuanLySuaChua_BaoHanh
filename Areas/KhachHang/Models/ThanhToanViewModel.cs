using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Models
{
    public class ThanhToanViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền cần thanh toán")]
        [Display(Name = "Số tiền thanh toán")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả đơn hàng")]
        [Display(Name = "Nội dung thanh toán")]        public string? OrderDescription { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }
        
        [Display(Name = "Mã phiếu sửa chữa")]
        public string? PhieuSuaChuaId { get; set; }
        
        [Display(Name = "Tên sản phẩm")]
        public string? TenSanPham { get; set; }
        
        [Display(Name = "Thông tin dịch vụ")]
        public string? ThongTinDichVu { get; set; }
        
        public string? NgayGui { get; set; }
        
        // Thêm các trường mới cho các phương thức thanh toán
        [Display(Name = "Phương thức thanh toán")]
        public string? PaymentMethod { get; set; }
        
        [Display(Name = "Mã ngân hàng")]
        public string? BankCode { get; set; }
        
        // Thông tin thẻ quốc tế
        [Display(Name = "Số thẻ")]
        public string? CardNumber { get; set; }
        
        [Display(Name = "Tên chủ thẻ")]
        public string? CardName { get; set; }
        
        [Display(Name = "Ngày hết hạn")]
        public string? CardExpiry { get; set; }
        
        [Display(Name = "Mã CVV/CVC")]
        public string? CardCvv { get; set; }
    }
}
