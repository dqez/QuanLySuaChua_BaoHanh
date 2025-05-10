using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;

public static class RoleSeedData
{
    public static async Task SeedRoles(RoleManager<IdentityRole<string>> roleManager, IDGenerator idGenerator)
    {
        string[] roleNames = { "QuanTriVien", "KhachHang", "KyThuatVien", "NhanVienKho", "TuVanVien" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                string roleId = await idGenerator.GenerateRoleIdAsync(roleName);

                var role = new IdentityRole<string>
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper()
                };
                await roleManager.CreateAsync(role);
            }
        }
    }

    public static async Task SeedAdminUser(UserManager<NguoiDung> userManager, IDGenerator idGenerator, BHSC_DbContext context)
    {
        if (await userManager.FindByNameAsync("admin1") == null)
        {
            var phuong = await context.Phuongs.FindAsync("PH001");

            if (phuong == null)
            {
                phuong = new Phuong
                {
                    PhuongId = "PH001",
                    TenPhuong = "Phường Mặc Định",
                    QuanId = "QU001"
                };
                context.Phuongs.Add(phuong);
                await context.SaveChangesAsync();
            }

            var adminId = await idGenerator.GenerateNguoiDungIDAsync("QuanTriVien");

            var admin = new NguoiDung
            {
                Id = adminId,
                UserName = "admin1",
                Email = "admin@example.com",
                NormalizedUserName = "ADMIN1",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                HoTen = "Quản trị viên",
                PhoneNumber = "0123456789",
                PhoneNumberConfirmed = true,
                PhuongId = "PH001",
                VaiTro = "QuanTriVien",
                DiaChi = "Địa chỉ văn phòng",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "QuanTriVien");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Không thể tạo người dùng quản trị: {errors}");
            }
        }
    }
}