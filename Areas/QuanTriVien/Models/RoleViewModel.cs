using System.ComponentModel.DataAnnotations;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống!")]
        [Display(Name = "Tên vai trò")]
        public string Name { get; set; }
    }

    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> SelectedRoles { get; set; }
    }
}