//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using QuanLySuaChua_BaoHanh.Models;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Models;

//namespace QuanLySuaChua_BaoHanh.Areas.QuanTriVien.Controllers
//{
//    [Area("QuanTriVien")]
//    [Authorize(Roles = "QuanTriVien")]
//    public class UserController : Controller
//    {
//        private readonly UserManager<NguoiDung> _userManager;
//        private readonly RoleManager<IdentityRole<int>> _roleManager;
//        private readonly BHSC_DbContext _context;

//        public UserController(
//            UserManager<NguoiDung> userManager,
//            RoleManager<IdentityRole<int>> roleManager,
//            BHSC_DbContext context)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _context = context;
//        }

//        // get: quantrivien/user
//        public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
//        {
//            if (searchString != null)
//            {
//                pageNumber = 1;
//            }
//            else
//            {
//                searchString = currentFilter;
//            }

//            ViewData["CurrentFilter"] = searchString;
//            var users = _userManager.Users.Include(u => u.Phuong);

//            if (!String.IsNullOrEmpty(searchString))
//            {
//                users = users.Where(u =>
//                    u.UserName.Contains(searchString) ||
//                    u.Email.Contains(searchString) ||
//                    (u.HoTen != null && u.HoTen.Contains(searchString)) ||
//                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchString))
//                );
//            }

//            int pageSize = 10;
//            return View(await PaginatedList<NguoiDung>.CreateAsync(users.AsNoTracking(), pageNumber ?? 1, pageSize));
//        }

//        // GET: Admin/User/Details/5
//        public async Task<IActionResult> Details(int id)
//        {
//            var user = await _userManager.FindByIdAsync(id.ToString());
//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Lấy danh sách role của user
//            var userRoles = await _userManager.GetRolesAsync(user);
//            ViewBag.UserRoles = userRoles;

//            // Lấy thông tin phường, quận, thành phố
//            if (user.PhuongId > 0)
//            {
//                var phuong = await _context.Phuongs
//                    .Include(p => p.Quan)
//                    .ThenInclude(q => q.ThanhPho)
//                    .FirstOrDefaultAsync(p => p.PhuongId == user.PhuongId);

//                if (phuong != null)
//                {
//                    ViewBag.DiaChi = $"{phuong.TenPhuong}, {phuong.Quan.TenQuan}, {phuong.Quan.ThanhPho.TenThanhPho}";
//                }
//            }

//            return View(user);
//        }

//        // GET: Admin/User/Create
//        public async Task<IActionResult> Create()
//        {
//            var roles = await _roleManager.Roles.ToListAsync();
//            ViewBag.Roles = roles;

//            // Lấy danh sách phường, quận, thành phố
//            var phuongs = await _context.Phuongs
//                .Include(p => p.Quan)
//                .ThenInclude(q => q.ThanhPho)
//                .ToListAsync();

//            ViewBag.PhuongId = new SelectList(phuongs, "PhuongId",
//                p => $"{p.TenPhuong}, {p.Quan.TenQuan}, {p.Quan.ThanhPho.TenThanhPho}");

//            return View();
//        }

//        // POST: Admin/User/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(CreateUserViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var user = new NguoiDung
//                {
//                    UserName = model.UserName,
//                    Email = model.Email,
//                    HoTen = model.HoTen,
//                    PhoneNumber = model.PhoneNumber,
//                    DiaChi = model.DiaChi,
//                    PhuongId = model.PhuongId,
//                    EmailConfirmed = true,
//                    PhoneNumberConfirmed = true
//                };

//                var result = await _userManager.CreateAsync(user, model.Password);
//                if (result.Succeeded)
//                {
//                    // Gán role cho user mới
//                    if (model.SelectedRoles != null && model.SelectedRoles.Count > 0)
//                    {
//                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
//                    }

//                    TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
//                    return RedirectToAction(nameof(Index));
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }

//            // Nếu không thành công, hiển thị lại form
//            var roles = await _roleManager.Roles.ToListAsync();
//            ViewBag.Roles = roles;

//            var phuongs = await _context.Phuongs
//                .Include(p => p.Quan)
//                .ThenInclude(q => q.ThanhPho)
//                .ToListAsync();

//            ViewBag.PhuongId = new SelectList(phuongs, "PhuongId",
//                p => $"{p.TenPhuong}, {p.Quan.TenQuan}, {p.Quan.ThanhPho.TenThanhPho}", model.PhuongId);

//            return View(model);
//        }

//        // GET: Admin/User/Edit/5
//        public async Task<IActionResult> Edit(int id)
//        {
//            var user = await _userManager.FindByIdAsync(id.ToString());
//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Lấy danh sách role của user
//            var userRoles = await _userManager.GetRolesAsync(user);
//            var roles = await _roleManager.Roles.ToListAsync();

//            var model = new EditUserViewModel
//            {
//                Id = user.Id,
//                UserName = user.UserName,
//                Email = user.Email,
//                HoTen = user.HoTen,
//                PhoneNumber = user.PhoneNumber,
//                DiaChi = user.DiaChi,
//                PhuongId = user.PhuongId,
//                SelectedRoles = userRoles.ToList()
//            };

//            ViewBag.Roles = roles;

//            var phuongs = await _context.Phuongs
//                .Include(p => p.Quan)
//                .ThenInclude(q => q.ThanhPho)
//                .ToListAsync();

//            ViewBag.PhuongId = new SelectList(phuongs, "PhuongId",
//                p => $"{p.TenPhuong}, {p.Quan.TenQuan}, {p.Quan.ThanhPho.TenThanhPho}", user.PhuongId);

//            return View(model);
//        }

//        // POST: Admin/User/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
//        {
//            if (id != model.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                var user = await _userManager.FindByIdAsync(id.ToString());
//                if (user == null)
//                {
//                    return NotFound();
//                }

//                user.Email = model.Email;
//                user.HoTen = model.HoTen;
//                user.PhoneNumber = model.PhoneNumber;
//                user.DiaChi = model.DiaChi;
//                user.PhuongId = model.PhuongId;

//                var result = await _userManager.UpdateAsync(user);
//                if (result.Succeeded)
//                {
//                    // Cập nhật mật khẩu nếu có
//                    if (!string.IsNullOrEmpty(model.Password))
//                    {
//                        // Xóa password hiện tại và đặt password mới
//                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
//                        var resetPassResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

//                        if (!resetPassResult.Succeeded)
//                        {
//                            foreach (var error in resetPassResult.Errors)
//                            {
//                                ModelState.AddModelError("", error.Description);
//                            }
//                            return View(model);
//                        }
//                    }

//                    // Cập nhật roles
//                    var userRoles = await _userManager.GetRolesAsync(user);
//                    await _userManager.RemoveFromRolesAsync(user, userRoles);

//                    if (model.SelectedRoles != null && model.SelectedRoles.Count > 0)
//                    {
//                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
//                    }

//                    TempData["SuccessMessage"] = "Cập nhật tài khoản thành công!";
//                    return RedirectToAction(nameof(Index));
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }

//            // Nếu không thành công, hiển thị lại form
//            var roles = await _roleManager.Roles.ToListAsync();
//            ViewBag.Roles = roles;

//            var phuongs = await _context.Phuongs
//                .Include(p => p.Quan)
//                .ThenInclude(q => q.ThanhPho)
//                .ToListAsync();

//            ViewBag.PhuongId = new SelectList(phuongs, "PhuongId",
//                p => $"{p.TenPhuong}, {p.Quan.TenQuan}, {p.Quan.ThanhPho.TenThanhPho}", model.PhuongId);

//            return View(model);
//        }

//        // GET: Admin/User/Delete/5
//        public async Task<IActionResult> Delete(int id)
//        {
//            var user = await _userManager.FindByIdAsync(id.ToString());
//            if (user == null)
//            {
//                return NotFound();
//            }

//            var roles = await _userManager.GetRolesAsync(user);
//            ViewBag.UserRoles = roles;

//            return View(user);
//        }

//        // POST: Admin/User/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var user = await _userManager.FindByIdAsync(id.ToString());
//            if (user == null)
//            {
//                return NotFound();
//            }

//            var result = await _userManager.DeleteAsync(user);
//            if (result.Succeeded)
//            {
//                TempData["SuccessMessage"] = "Xóa tài khoản thành công!";
//                return RedirectToAction(nameof(Index));
//            }

//            foreach (var error in result.Errors)
//            {
//                TempData["ErrorMessage"] = error.Description;
//            }

//            return RedirectToAction(nameof(Delete), new { id = id });
//        }
//    }

//    // Class hỗ trợ phân trang
//    public class PaginatedList<T> : List<T>
//    {
//        public int PageIndex { get; private set; }
//        public int TotalPages { get; private set; }

//        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
//        {
//            PageIndex = pageIndex;
//            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

//            this.AddRange(items);
//        }

//        public bool HasPreviousPage => PageIndex > 1;
//        public bool HasNextPage => PageIndex < TotalPages;

//        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
//        {
//            var count = await source.CountAsync();
//            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
//            return new PaginatedList<T>(items, count, pageIndex, pageSize);
//        }
//    }
//}
