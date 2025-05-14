using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySuaChua_BaoHanh.Areas.NhanVienKho.Controllers
{
    public class HomeController : Controller
    {
        [Area("NhanVienKho")]
        [Authorize(Roles = "NhanVienKho")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
