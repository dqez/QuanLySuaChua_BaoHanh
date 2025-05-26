using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuanLySuaChua_BaoHanh.Enums
{
    public enum TrangThaiPhieu
    {
        //Khachhang đăng ký phiếu sửa chữa
        [Display(Name = "Chờ xác nhận")] 
        ChoXacNhan,
        
        //tư vấn viên xác nhận
        [Display(Name = "Đã xác nhận")]
        DaXacNhan,
        
        //quản trị viên phân công (KythuatID) 
        [Display(Name = "Đã phân công")] 
        DaPhanCong,

        //kỹ thuât viên chọn ngày hẹn
        [Display(Name = "Chờ kiểm tra")]
        ChoKiemTra,

        // //kythuatvien xác nhận, không -> đã hủy
        // [Display(Name = "Khách đồng ý sửa")] 
        // KhachDongY,
        
        //ky thuat viên cập nhật linh kiện 
        [Display(Name = "Đang sửa chữa")]
        DangSuaChua,
        
        //ky thuât viên cập nhật trạng thái sửa chữa
        [Display(Name = "Đã sửa xong")] 
        DaSuaXong,

        //khachhang thanh toán.
        [Display(Name = "Đã thanh toán")]
        DaThanhToan,

        //Quản trị viên cập nhật sau khi đóng gói và giao cho bên vận chuyển.
        [Display(Name = "Đang vận chuyển")]
        DangVanChuyen,

        //khachhnag xác nhận nhận hàng.
        [Display(Name = "Hoàn thành")]
        HoanThanh,


        //Khachhnag không đồng ý -> KTV cập nhật đã hủy.
        [Display(Name = "Đã hủy")]
        DaHuy
    }

    public static class TrangThaiPhieuExtension
    {
        public static string GetDisplayName(this TrangThaiPhieu trangThai)
        {
            var field = trangThai.GetType().GetField(trangThai.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? trangThai.ToString();
        }
    }


    //cách dùng: thay thế  "<td>@Html.DisplayFor(modelItem => item.TrangThai)</td>" trong file cshtml bằng code dưới

    //              <td>
    //                  @{
    //                      var trangThaiEnum = Enum.TryParse<QuanLySuaChua_BaoHanh.Enums.TrangThaiPhieu>(item.TrangThai, out var t) ? t : default;
    //                   }
    //                  @trangThaiEnum.GetDisplayName()
    //              </td >

}