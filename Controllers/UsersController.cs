

using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View(new UserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                NationalId = model.NationalId,
                NormalizedUserName = model.UserName.ToUpper(),
                Email = model.Email,
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                CreatedById = "Admin",
                RoleId = model.RoleId
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                TempData["Message"] = "User created successfully";
                return RedirectToAction("Index");
            }

            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new UserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                NationalId = user.NationalId,
                RoleId = user.RoleId
            };

            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserViewModel model)
        {
            if (id != model.Id) // أو اجعل model.Id string
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.NationalId = model.NationalId;
            user.RoleId = model.RoleId;
            user.ModifiedById = User.Identity.Name;
            user.ModifiedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name", model.RoleId);
                return View(model);
            }

            TempData["Message"] = "User updated successfully";
            return RedirectToAction("Edit");
        }



    }
}

