using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Enums
{
    public enum TrangThaiPhieu
    {
        [Display(Name = "Chờ xác nhận")]
        ChoXacNhan,

        [Display(Name = "Đã tiếp nhận")]
        DaTiepNhan,

        [Display(Name = "Đang sửa chữa")]
        DangSuaChua,

        [Display(Name = "Hoàn thành")]
        HoanThanh,

        [Display(Name = "Đã hủy")]
        DaHuy
    }
}
