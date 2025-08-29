using EmployeesManagment.Data;
using EmployeesManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class ApprovalsUserMatricesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalsUserMatricesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApprovalsUserMatrices
        public async Task<IActionResult> Index()
        {
            var matrix =await _context.ApprovalsUserMatrixes
                .Include(a => a.DocumentType)
                .Include(a => a.User)
                .Include(a => a.WorkFlowUserGroup).ToListAsync();
            return View(matrix);
        }

        // GET: ApprovalsUserMatrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalsUserMatrix = await _context.ApprovalsUserMatrixes
                .Include(a => a.DocumentType)
                .Include(a => a.User)
                .Include(a => a.WorkFlowUserGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalsUserMatrix == null)
            {
                return NotFound();
            }

            return View(approvalsUserMatrix);
        }

        // GET: ApprovalsUserMatrices/Create
        public IActionResult Create()
        {
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["workFlowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description");
            return View();
        }

        // POST: ApprovalsUserMatrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApprovalsUserMatrix approvalsUserMatrix)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                approvalsUserMatrix.CreatedById = User.Identity.Name;
                approvalsUserMatrix.CreatedOn = DateTime.Now;
                approvalsUserMatrix.ModifiedOn = DateTime.Now;
                approvalsUserMatrix.ModifiedById= User.Identity.Name;
                _context.Add(approvalsUserMatrix);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCodeValue).Where(y=>y.SystemCodeValue.Code=="DocumentTypes"), "Id", "Description", approvalsUserMatrix.DocumentTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalsUserMatrix.UserId);
            ViewData["workFlowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalsUserMatrix.workFlowUserGroupId);
            return View(approvalsUserMatrix);
        }

        // GET: ApprovalsUserMatrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalsUserMatrix = await _context.ApprovalsUserMatrixes.FindAsync(id);
            if (approvalsUserMatrix == null)
            {
                return NotFound();
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "DocumentTypes"), "Id", "Description", approvalsUserMatrix.DocumentTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalsUserMatrix.UserId);
            ViewData["workFlowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalsUserMatrix.workFlowUserGroupId);
            return View(approvalsUserMatrix);
        }

        // POST: ApprovalsUserMatrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ApprovalsUserMatrix approvalsUserMatrix)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != approvalsUserMatrix.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    approvalsUserMatrix.ModifiedById = User.Identity.Name;
                    approvalsUserMatrix.ModifiedOn = DateTime.Now;
                    _context.Update(approvalsUserMatrix);
                    await _context.SaveChangesAsync(userId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalsUserMatrixExists(approvalsUserMatrix.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "DocumentTypes"), "Id", "Description", approvalsUserMatrix.DocumentTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", approvalsUserMatrix.UserId);
            ViewData["workFlowUserGroupId"] = new SelectList(_context.WorkFlowUserGroups, "Id", "Description", approvalsUserMatrix.workFlowUserGroupId);
            return View(approvalsUserMatrix);
        }

        // GET: ApprovalsUserMatrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalsUserMatrix = await _context.ApprovalsUserMatrixes
                .Include(a => a.DocumentType)
                .Include(a => a.User)
                .Include(a => a.WorkFlowUserGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalsUserMatrix == null)
            {
                return NotFound();
            }

            return View(approvalsUserMatrix);
        }

        // POST: ApprovalsUserMatrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var approvalsUserMatrix = await _context.ApprovalsUserMatrixes.FindAsync(id);
            if (approvalsUserMatrix != null)
            {
                _context.ApprovalsUserMatrixes.Remove(approvalsUserMatrix);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalsUserMatrixExists(int id)
        {
            return _context.ApprovalsUserMatrixes.Any(e => e.Id == id);
        }
    }
}
