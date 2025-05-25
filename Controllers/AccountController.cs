using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;
using QuanLySuaChua_BaoHanh.ViewModels;

namespace QuanLySuaChua_BaoHanh.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<NguoiDung> _userManager;
        private readonly SignInManager<NguoiDung> _signInManager;
        private readonly BHSC_DbContext _context;
        private readonly IDGenerator _idGenerator;


        public AccountController(
            UserManager<NguoiDung> userManager,
            SignInManager<NguoiDung> signInManager,
            BHSC_DbContext context,
            IDGenerator idGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _idGenerator = idGenerator;
        }



        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Phuongs = new SelectList(await _context.Phuongs.ToListAsync(), "PhuongId", "TenPhuong");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new NguoiDung
                {
                    Id = await _idGenerator.GenerateNguoiDungIDAsync("KhachHang"),
                    UserName = model.UserName,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    PhoneNumber = model.PhoneNumber,
                    DiaChi = model.DiaChi,
                    PhuongId = model.PhuongId.ToString(),
                    VaiTro = "KhachHang"
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "KhachHang");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new { area = "KhachHang"});
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Phuongs = new SelectList(await _context.Phuongs.ToListAsync(), "PhuongId", "TenPhuong");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                // string[] roleNames = { "QuanTriVien", "KhachHang", "KyThuatVien", "NhanVienKho", "TuVanVien" };
                if (await _userManager.IsInRoleAsync(user, "KhachHang"))
                {
                    return RedirectToAction("Index", "Home", new { area = "QuanTriVien" });
                }
                else if (await _userManager.IsInRoleAsync(user, "KyThuatVien"))
                {
                    return RedirectToAction("Index", "Home", new { area = "KyThuatVien" });
                }
                else if (await _userManager.IsInRoleAsync(user, "NhanVienKho"))
                {
                    return RedirectToAction("Index", "Home", new { area = "NhanVienKho" });
                }
                else if (await _userManager.IsInRoleAsync(user, "TuVanVien"))
                {
                    return RedirectToAction("Index", "Home", new { area = "TuVanVien" });
                }
                else if (await _userManager.IsInRoleAsync(user, "QuanTriVien"))
                {
                    return RedirectToAction("Index", "Home", new { area = "KhachHang" });
                }
                else
                {
                    return RedirectToAction("index", "home");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    //Điều hướng đến trang tương ứng với vai trò của người dùng
                    if (await _userManager.IsInRoleAsync(user, "QuanTriVien"))
                        return RedirectToAction("Index", "Home", new { area = "QuanTriVien" });
                    if (await _userManager.IsInRoleAsync(user, "KhachHang"))
                        return RedirectToAction("Index", "Home", new { area = "KhachHang" });
                    if (await _userManager.IsInRoleAsync(user, "KyThuatVien"))
                        return RedirectToAction("Index", "Home", new { area = "KyThuatVien" });
                    if (await _userManager.IsInRoleAsync(user, "NhanVienKho"))
                        return RedirectToAction("Index", "Home", new { area = "NhanVienKho" });
                    if (await _userManager.IsInRoleAsync(user, "TuVanVien"))
                        return RedirectToAction("Index", "Home", new { area = "TuVanVien" });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Đăng nhập không thành công.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete(".AspNetCore.Identity.Application");
            Response.Cookies.Delete(".AspNetCore.Cookies");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin phường, quận, thành phố
            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(p => p.PhuongId.ToString() == user.PhuongId);            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                HoTen = user.HoTen,
                PhoneNumber = user.PhoneNumber,
                DiaChi = user.DiaChi,
                PhuongId = user.PhuongId,
                VaiTro = user.VaiTro,
                TenPhuong = phuong?.TenPhuong,
                TenQuan = phuong?.Quan?.TenQuan,
                TenThanhPho = phuong?.Quan?.ThanhPho?.TenThanhPho,
                ThanhPhoId = phuong?.Quan?.ThanhPho?.ThanhPhoId.ToString(),
                QuanId = phuong?.Quan?.QuanId.ToString()
            };

            return View(viewModel);
        }        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Lấy thông tin phường, quận, thành phố hiện tại
            var phuong = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(p => p.PhuongId.ToString() == user.PhuongId);

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                HoTen = user.HoTen,
                PhoneNumber = user.PhoneNumber,
                DiaChi = user.DiaChi,
                PhuongId = user.PhuongId,
                VaiTro = user.VaiTro,
                ThanhPhoId = phuong?.Quan?.ThanhPho?.ThanhPhoId.ToString(),
                QuanId = phuong?.Quan?.QuanId.ToString()
            };

            // Set ViewBag for selected values
            ViewBag.SelectedThanhPhoId = phuong?.Quan?.ThanhPho?.ThanhPhoId.ToString();
            ViewBag.SelectedQuanId = phuong?.Quan?.QuanId.ToString();
            ViewBag.SelectedPhuongId = user.PhuongId;

            return View(viewModel);
        }        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Set ViewBag for selected values when validation fails
                var phuong = await _context.Phuongs
                    .Include(p => p.Quan)
                    .ThenInclude(q => q.ThanhPho)
                    .FirstOrDefaultAsync(p => p.PhuongId.ToString() == model.PhuongId);
                
                ViewBag.SelectedThanhPhoId = phuong?.Quan?.ThanhPho?.ThanhPhoId.ToString();
                ViewBag.SelectedQuanId = phuong?.Quan?.QuanId.ToString();
                ViewBag.SelectedPhuongId = model.PhuongId;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Cập nhật thông tin người dùng - chỉ cập nhật PhuongId
            user.Email = model.Email;
            user.HoTen = model.HoTen;
            user.PhoneNumber = model.PhoneNumber;
            user.DiaChi = model.DiaChi;
            user.PhuongId = model.PhuongId; // Chỉ lưu PhuongId

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Set ViewBag for selected values when update fails
            var phuongForError = await _context.Phuongs
                .Include(p => p.Quan)
                .ThenInclude(q => q.ThanhPho)
                .FirstOrDefaultAsync(p => p.PhuongId.ToString() == model.PhuongId);
            
            ViewBag.SelectedThanhPhoId = phuongForError?.Quan?.ThanhPho?.ThanhPhoId.ToString();
            ViewBag.SelectedQuanId = phuongForError?.Quan?.QuanId.ToString();
            ViewBag.SelectedPhuongId = model.PhuongId;
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (changePasswordResult.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Profile");
            }

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
