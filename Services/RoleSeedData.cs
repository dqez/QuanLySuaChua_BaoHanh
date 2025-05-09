using Microsoft.AspNetCore.Identity;
using QuanLySuaChua_BaoHanh.Models;

public static class RoleSeedData
{
    public static async Task SeedRoles(RoleManager<IdentityRole<string>> roleManager)
    {
        string[] roleNames = { "QuanTriVien", "KhachHang", "KyThuatVien", "NhanVienKho", "TuVanVien" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<string>(roleName));
            }
        }
    }

    public static async Task SeedAdminUser(UserManager<NguoiDung> userManager)
    {
        if (await userManager.FindByNameAsync("admin1") == null)
        {
            var admin = new NguoiDung
            {
                UserName = "admin1",
                Email = "admin@example.com",
                HoTen = "Quản trị viên",
                PhoneNumber = "0123456789",
                PhuongId = "HC",
                VaiTro = "QuanTriVien",
                DiaChi = "Địa chỉ văn phòng"
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "QuanTriVien");
            }
        }
    }
}