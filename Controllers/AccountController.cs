using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);
                return RedirectToAction("Landing", "Home");
            }

            ViewBag.ErrorMessage = "Inva lid login attempt";
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                var existing = await _userManager.FindByEmailAsync(model.Email);
                if (existing != null)
                {
                    TempData["Error"] = "User could be exist already ";
                    return View(model);
                }

                // تقسيم الاسم
                var nameParts = (model.FullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),

                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpper(),
                    EmailConfirmed = true, // بما أنك لا تستخدم تأكيد إيميل الآن

                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,

                    FirstName = nameParts.Length > 0 ? nameParts[0] : "",
                    MiddleName = nameParts.Length > 2 ? nameParts[1] : "",
                    LastName = nameParts.Length > 1 ? nameParts[^1] : "",

                    NationalId = model.NationalId,

                    CreatedOn = DateTime.Now,
                    CreatedById = "Self",

                    RoleId = await GetDefaultRoleId() // سيتم شرحه بالأسفل
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // إضافة المستخدم لدور User
                    var role = await _roleManager.FindByIdAsync(user.RoleId);
                    await _userManager.AddToRoleAsync(user, role.Name);

                    // تسجيل الدخول فوراً
                    await _signInManager.SignInAsync(user, false);
                    TempData["Message"] = "User Created Successfuly";
                    return RedirectToAction("Landing", "Home");
                }
            }
            

            catch (Exception ex)
            {
                TempData["Error"] = "User could not created " + ex.Message;

            }

            return View(model);
        }

        private async Task<string> GetDefaultRoleId()
        {
            var role = await _roleManager.FindByNameAsync("User");

            // إنشاء دور User اذا لم يكن موجوداً
            if (role == null)
            {
                role = new IdentityRole("User");
                await _roleManager.CreateAsync(role);
            }
            return role.Id;
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
       
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

    }
}
