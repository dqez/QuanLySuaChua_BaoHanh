using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using System.Threading.Tasks;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class ManageAccountsController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public ManageAccountsController(
            UserManager<NguoiDung> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Take(5)
                .ToListAsync();
                
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Take(5)
                .ToListAsync();

            var model = new AccountsOverviewViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(model);
        }
    }

    public class AccountsOverviewViewModel
    {
        public IEnumerable<NguoiDung> Users { get; set; }
        public IEnumerable<IdentityRole<int>> Roles { get; set; }
    }
}
