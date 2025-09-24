using EmployeesManagment.Data;
using EmployeesManagment.Data.Seed;
using EmployeesManagment.Hubs;
using EmployeesManagment.Infrastructure;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.Views.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
var builder = WebApplication.CreateBuilder(args);

// ----------------- Add Services -----------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity configuration with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Razor Pages & MVC
builder.Services.AddRazorPages();
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LicenseFilter>();
});
builder.Services.AddScoped<LicenseService>();
builder.Services.AddSession();
// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
// ✅ إضافة SignalR
builder.Services.AddSignalR();

// ✅ UserIdProvider
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// ✅ NotificationService
builder.Services.AddScoped<NotificationService>();
// AutoMapper
var config = new AutoMapper.MapperConfiguration(options =>
{
    options.AllowNullDestinationValues = true;
    options.AllowNullCollections = true;
    options.AddProfile(new AutomapperProfiles());
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

// Custom services
builder.Services.AddTransient<IExtensionService, ExtensionService>();

// Reporting service
builder.Services.AddScoped<EmployeesManagment.Services.Reporting.IReportService, EmployeesManagment.Services.Reporting.ReportService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});



// ضبط الترخيص لمرة واحدة فقط
ExcelPackage.License.SetNonCommercialPersonal("Your Name Here");
builder.Services.AddControllersWithViews();

// ----------------- Build App -----------------
var app = builder.Build();
app.UseSession();


//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//    await DbSeeder.SeedAdminUserAsync(userManager, roleManager);
//}
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndUsersAsync(sp);
    await FakeDataSeeder.SeedAsync(sp);
}


// ----------------- Configure Middleware -----------------
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
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<NotificationHub>("/notificationHub");


// ----------------- Run App -----------------
app.Run();
