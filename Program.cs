using FreightManagement.Data;
using FreightManagement.Repositories.RepoInterfaces;
using FreightManagement.Repositories.Repository;
using FreightManagement.Services.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FreightManagement.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── YC1: Thay Session bằng Cookie Authentication ──────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddAuthorization();

// ── Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();

// ── Repositories ──────────────────────────────────────────────────
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IDonHangsRepository, DonHangsRepository>();
builder.Services.AddScoped<IHangTrongKhosRepository, HangTrongKhosRepository>();
builder.Services.AddScoped<IKhoHangsRepository, KhoHangsRepository>();
builder.Services.AddScoped<ILichSuTrangThaisRepository, LichSuTrangThaisRepository>();
builder.Services.AddScoped<IThongKeDoanhThusRepository, ThongKeDoanhThusRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();