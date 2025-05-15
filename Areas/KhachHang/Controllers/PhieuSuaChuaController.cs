using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Services;
using QuanLySuaChua_BaoHanh.Models;

namespace QuanLySuaChua_BaoHanh.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize(Roles = "KhachHang")]
    public class PhieuSuaChuaController : Controller
    {
        private readonly UserManager<NguoiDung> _userManager;
        private readonly IPhieuSuaChuaService _phieuSuaChuaService;

        public PhieuSuaChuaController(
            UserManager<NguoiDung> userManager, 
            IPhieuSuaChuaService phieuSuaChuaService)
        {
            _userManager = userManager;
            _phieuSuaChuaService = phieuSuaChuaService;
        }

        // GET: KhachHang/PhieuSuaChua/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var products = await _phieuSuaChuaService.GetUserProducts(user.Id);
            if (products.Count > 0)
            {
                ViewBag.Products = products;
            }
            return View();
        }

        // POST: KhachHang/PhieuSuaChua/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuSuaChuaVM phieuSuaChuaVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                var (success, message) = await _phieuSuaChuaService.CreatePhieuSuaChua(phieuSuaChuaVM, user.Id);
                if (success)
                {
                    TempData["Success"] = message;
                }
                else
                {
                    TempData["Error"] = message;
                }
                return RedirectToAction("Create");
            }

            return View(phieuSuaChuaVM);
        }
    }
}
