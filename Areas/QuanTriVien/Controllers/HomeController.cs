using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
{     
    [Area("QuanTriVien")]
    [Authorize(Roles = "QuanTriVien")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
