using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models
{
    public class PhuongViewModel
    {
        public Phuong Phuong { get; set; }
        public IEnumerable<SelectListItem> ThanhPhos { get; set; }
        public IEnumerable<SelectListItem> Quans { get; set; }
    }
}
