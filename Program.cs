using FreightManagement.Data;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.MainRepositories.Repository;
using FreightManagement.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<WarehouseService>();

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