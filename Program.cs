using FreightManagement.Data;
using FreightManagement.MainRepositories.RepoInterfaces;
using FreightManagement.MainRepositories.Repository;
using FreightManagement.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<WarehouseService>();

builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<DonHangsRepository>();
builder.Services.AddScoped<HangTrongKhosRepository>();
builder.Services.AddScoped<LichSuTrangThaisRepository>();
builder.Services.AddScoped<ThongKeDoanhThusRepository>();
builder.Services.AddScoped<KhoHangsRepository>();

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
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();