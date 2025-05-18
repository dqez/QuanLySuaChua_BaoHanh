using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Tên vai trò không được để trống!")]
        [Display(Name = "Tên vai trò")]
        public string Name { get; set; } = null!;
    }

    public class UserRolesViewModel
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public IList<string> Roles { get; set; } = new List<string>();
        public IList<string> SelectedRoles { get; set; } = new List<string>();
    }
}
