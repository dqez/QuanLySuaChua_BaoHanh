using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models
{
    public class ImportViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn file")]
        [Display(Name = "File Excel/CSV")]
        public IFormFile File { get; set; }
        
        public string ImportType { get; set; }
    }
} 