using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLySuaChua_BaoHanh.Models;
using QuanLySuaChua_BaoHanh.Services;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using QuanLySuaChua_BaoHanh.Areas.KhachHang.Models;


var contextPdfAssemblyLoadContext = new CustomAssemblyLoadContext();
var wkhtmltoxPath = Path.Combine(Directory.GetCurrentDirectory(), "NativeLibrary", "libwkhtmltox.dll");
contextPdfAssemblyLoadContext.LoadUnmanagedLibrary(wkhtmltoxPath);

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BHSC_DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<NguoiDung,IdentityRole<string>>(options =>
    {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 1;
    })
    .AddEntityFrameworkStores<BHSC_DbContext>()
    .AddDefaultTokenProviders();

// Cấu hình VnPay
builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection("VnPay"));

builder.Services.AddHttpClient<MapService>();

// Thêm memory cache nếu chưa có
builder.Services.AddMemoryCache();

//cau hinh cookie
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(1);
    options.Cookie.IsEssential = true; 
});

//dang ky IDGenerator
builder.Services.AddScoped<IDGenerator>();

// Đăng ký service PhieuSuaChua
builder.Services.AddScoped<IPhieuSuaChuaService, PhieuSuaChuaService>();

// Đăng ký service Export
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IExportService, ExportService>();

// Đăng ký service Excel Import
builder.Services.AddScoped<ExcelImportService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Khởi tạo dữ liệu cho các vai trò và người dùng
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<string>>>();
        var userManager = services.GetRequiredService<UserManager<NguoiDung>>();
        var idGenerator = services.GetRequiredService<IDGenerator>();
        var context = services.GetRequiredService<BHSC_DbContext>();

        await RoleSeedData.SeedRoles(roleManager, idGenerator);
        await RoleSeedData.SeedAdminUser(userManager, idGenerator, context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles and users.");
    }
}

app.Run();