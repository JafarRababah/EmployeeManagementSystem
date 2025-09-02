using EmployeesManagment.Data;
using EmployeesManagment.Data.Seed;
using EmployeesManagment.Infrastructure;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.Views.Profiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------- Add Services -----------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity configuration with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Razor Pages & MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Authentication & Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

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

// ----------------- Build App -----------------
var app = builder.Build();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



// ----------------- Run App -----------------
app.Run();
