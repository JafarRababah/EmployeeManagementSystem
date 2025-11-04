using EmployeesManagment.Data;
using EmployeesManagment.Data.Seed;
using EmployeesManagment.Hubs;
using EmployeesManagment.Infrastructure;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.Views.Profiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Add Services -----------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ إعداد الكوكي فقط (بدون AddAuthentication)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// Razor Pages & MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LicenseFilter>();
});

builder.Services.AddScoped<LicenseService>();
builder.Services.AddSession();

// ✅ SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<NotificationService>();

// ✅ AutoMapper
var config = new AutoMapper.MapperConfiguration(options =>
{
    options.AddProfile(new AutomapperProfiles());
});
builder.Services.AddSingleton(config.CreateMapper());

// ✅ Custom services
builder.Services.AddTransient<IExtensionService, ExtensionService>();
builder.Services.AddScoped<EmployeesManagment.Services.Reporting.IReportService, EmployeesManagment.Services.Reporting.ReportService>();


// ✅ EPPlus License
ExcelPackage.License.SetNonCommercialPersonal("Your Name Here");

// ----------------- Build App -----------------
var app = builder.Build();

// ----------------- Middleware -----------------
app.Use(async (context, next) =>
{
    // منع عرض الصفحات من الكاش بعد تسجيل الخروج
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";
    await next();
});

app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndUsersAsync(sp);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ----------------- Routes -----------------
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Landing}/{id?}");


app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");

// ----------------- Run -----------------
app.Run();
