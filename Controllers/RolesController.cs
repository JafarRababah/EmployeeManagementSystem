using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmployeesManagment.Controllers
{
    public class RolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RoleViewModel()); // ✅ Now it matches the @model in Create.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            try
            {
                IdentityRole role = new IdentityRole();
                role.Name = model.RoleName;

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Role  created successfully ";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Error created Role ";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error created Role " + ex.Message;
                return View(model);
            }
        }
           
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role=new RoleViewModel();
            var result=await _roleManager.FindByIdAsync(id);
            role.RoleName = result.Name;
            role.Id = result.Id;
            return View(role); // ✅ Now it matches the @model in Create.cshtml
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id,RoleViewModel model)
        {
            var checkIfExist = await _roleManager.RoleExistsAsync(model.RoleName);

            var result = await _roleManager.FindByIdAsync(id);
            result.Name = model.RoleName;
            var finalResult=await _roleManager.UpdateAsync(result);
            if (checkIfExist) {
                if (finalResult.Succeeded)
                {
                    TempData["Message"] = "Role updated successfully ";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Error created Role ";
                    return View(model);
                }
            }
            return View(model);
        }
    }
}
