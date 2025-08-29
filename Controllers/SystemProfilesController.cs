using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using System.Security.Claims;

namespace EmployeesManagment.Controllers
{
    public class SystemProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemProfiles
        public async Task<IActionResult> Index()
        {
            var profiles = await _context.SystemProfile
                .Include(s => s.Profile)
                .ToListAsync();
            return View( profiles);
        }

        // GET: SystemProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfile
                .Include(s => s.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemProfile == null)
            {
                return NotFound();
            }

            return View(systemProfile);
        }

        // GET: SystemProfiles/Create
        public IActionResult Create()
        {
            ViewData["ProfileId"] = new SelectList(_context.SystemProfile, "Id", "Name");
            return View();
        }

        // POST: SystemProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemProfile systemProfile)
        {
            try
            {
                
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    systemProfile.CreatedById = User.Identity.Name;
                    systemProfile.CreatedOn = DateTime.Now;
                systemProfile.ModifiedOn = DateTime.Now;
                systemProfile.ModifiedById = userId;
                _context.Add(systemProfile);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "System profile created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                ViewData["ProfileId"] = new SelectList(_context.SystemProfile, "Id", "Name", systemProfile.ProfileId);
                TempData["Error"] = "Error created System Profile ";
                return View(systemProfile);
            }
        }

        // GET: SystemProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfile.FindAsync(id);
            if (systemProfile == null)
            {
                return NotFound();
            }
            ViewData["ProfileId"] = new SelectList(_context.SystemProfile, "Id", "Name", systemProfile.ProfileId);
            return View(systemProfile);
        }

        // POST: SystemProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SystemProfile systemProfile)
        {
            if (id != systemProfile.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var userId= User.FindFirstValue(ClaimTypes.NameIdentifier);
                    systemProfile.ModifiedOn = DateTime.Now;
                    systemProfile.ModifiedById = User.Identity.Name;
                    _context.Update(systemProfile);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "System profile updated successfully ";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemProfileExists(systemProfile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Error updated System Profile ";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProfileId"] = new SelectList(_context.SystemProfile, "Id", "Name", systemProfile.ProfileId);
            return View(systemProfile);
        }

        // GET: SystemProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemProfile = await _context.SystemProfile
                .Include(s => s.Profile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemProfile == null)
            {
                return NotFound();
            }

            return View(systemProfile);
        }

        // POST: SystemProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemProfile = await _context.SystemProfile.FindAsync(id);
            if (systemProfile != null)
            {
                _context.SystemProfile.Remove(systemProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemProfileExists(int id)
        {
            return _context.SystemProfile.Any(e => e.Id == id);
        }
    }
}
