using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace QuanLySuaChua_BaoHanh.Areas.TuVanVien.Controllers
{
    [Area("TuVanVien")]
    [Authorize(Roles = "TuVanVien")]
    public class CapNhatDonController : Controller
    {
        public IActionResult CapNhatDon()
        {
            return View("CapNhatDon");
        }
    }
}