using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using QuanLyDuAn.Data;
using QuanLyDuAn.Hubs;
using QuanLyDuAn.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext from appsettings.json
builder.Services.AddDbContext<BtlCnpmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BtlCnpm")));

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<ProjectHub>("/projectHub");

// Seeding Data on Startup
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BtlCnpmContext>();
        
        // Seed Goidichvu if empty
        if (!context.Goidichvus.Any())
        {
            context.Goidichvus.AddRange(
                new Goidichvu { MaGoi = "FREE", TenGoi = "Free Plan", Gia = 0, SoDuAnToiDa = 5, SoTvToiDa = 10, DungLuongMax = 1024, MoTa = "Gói miễn phí cho cá nhân và nhóm nhỏ" },
                new Goidichvu { MaGoi = "PRO", TenGoi = "Pro Plan", Gia = 150000, SoDuAnToiDa = 50, SoTvToiDa = 100, DungLuongMax = 10240, MoTa = "Gói chuyên nghiệp cho doanh nghiệp vừa" },
                new Goidichvu { MaGoi = "ENT", TenGoi = "Enterprise Plan", Gia = 500000, SoDuAnToiDa = 9999, SoTvToiDa = 9999, DungLuongMax = 102400, MoTa = "Gói không giới hạn cho tổ chức lớn" }
            );
            context.SaveChanges();
        }

        // Seed test admin user if empty
        if (!context.Taikhoans.Any(t => t.Email == "admin@projectflow.com"))
        {
            var hasher = new PasswordHasher<Taikhoan>();
            var adminUser = new Taikhoan
            {
                MaTaiKhoan = "ADMIN001",
                MaGoi = "FREE",
                HoTen = "Admin Test",
                Email = "admin@projectflow.com",
                SoDienThoai = "0123456789",
                VaiTro = "Admin",
                TrangThai = "KichHoat",
                NgayTao = QuanLyDuAn.Helpers.VnDateTime.Now
            };
            adminUser.MatKhau = hasher.HashPassword(adminUser, "admin123");
            context.Taikhoans.Add(adminUser);
            context.SaveChanges();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("Database seeding skipped or failed on startup: " + ex.Message);
}

app.Run();
