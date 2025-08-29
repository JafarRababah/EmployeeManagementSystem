using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tasks = new ProfileViewModel();
            var roles = await _context.Roles.OrderBy(x => x.Name).ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
           var systemTasks = await _context.SystemProfiles
                .Include("Children.Children.Children")
                .OrderBy(x => x.Order)
                .Where(t => t.ProfileId == null)
                .ToListAsync();
            ViewBag.Tasks = new SelectList(systemTasks, "Id", "Name");
            return View(tasks);
        }
        public async Task<ActionResult> AssignRights(ProfileViewModel vm)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var role = new RoleProfile
                {
                    TaskId = vm.TaskId,
                    RoleId = vm.RoleId,
                };
                _context.RoleProfiles.Add(role);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Role Assigning successfully ";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Assigning Role "+ex.Message;
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<IActionResult> UserRights(string id)
        {
            var tasks = new ProfileViewModel();
            tasks.RoleId = id;
            tasks.Profiles = await _context.SystemProfiles
                 .Include("Children.Children.Children")
                 .OrderBy(x => x.Order)
                 .ToListAsync();
            tasks.RolesRightsIds = await _context.RoleProfiles.Where(x => x.RoleId == id).Select(r => r.TaskId).ToListAsync();
            return View(tasks);
        }
    
        //[HttpPost]
        //public async Task<ActionResult> UserGroupRights(string id,ProfileViewModel vm)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var allRights = await _context.RoleProfiles.Where(x => x.RoleId == id).ToListAsync();
        //    _context.RoleProfiles.RemoveRange(allRights);
        //    await _context.SaveChangesAsync(userId);
        //    // تحقق من أن RoleId موجودة
        //    if (string.IsNullOrEmpty(vm.RoleId))
        //    {
        //        ModelState.AddModelError("RoleId", "RoleId is required.");
        //        return View(vm);
        //    }

        //    // تحقق من أن هناك مهام (Tasks) محددة
        //    if (vm.Ids != null && vm.Ids.Any())
        //    {
        //        foreach (var taskId in vm.Ids.Distinct())
        //        {
        //            var role = new RoleProfile
        //            {
        //                TaskId = taskId, // استخدم taskId من الحلقة وليس vm.TaskId
        //                RoleId = id
        //            };

        //            _context.RoleProfiles.Add(role);
        //            await _context.SaveChangesAsync(userId);
        //        }

               
        //    }

        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public async Task<ActionResult> UserGroupRights(ProfileViewModel vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var allRights = await _context.RoleProfiles.Where(x => x.RoleId == vm.RoleId).ToListAsync();
                _context.RoleProfiles.RemoveRange(allRights);
                await _context.SaveChangesAsync(userId);
                // تحقق من أن RoleId موجودة
                if (string.IsNullOrEmpty(vm.RoleId))
                {
                    ModelState.AddModelError("RoleId", "RoleId is required.");
                    return View(vm);
                }

                // تحقق من أن هناك مهام (Tasks) محددة
                if (vm.Ids != null && vm.Ids.Any())
                {
                    foreach (var taskId in vm.Ids.Distinct())
                    {
                        var role = new RoleProfile
                        {
                            TaskId = taskId, // استخدم taskId من الحلقة وليس vm.TaskId
                            RoleId = vm.RoleId
                        };

                        _context.RoleProfiles.Add(role);
                        await _context.SaveChangesAsync(userId);
                    }


                }
                TempData["Message"] = "Rights Assigned successfully ";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error Rights assigned " + ex.Message;
                return View(vm);
            }
        }

    }
}
