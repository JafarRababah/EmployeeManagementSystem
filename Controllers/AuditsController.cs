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
    public class AuditsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Audits
        public async Task<IActionResult> Index()
        {
            var auditLogs=await _context.AuditLogs
                .Include(x=>x.User)
                .ToListAsync();
            return View(auditLogs);
        }

        // GET: Audits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = await _context.AuditLogs
                .Include(x=>x.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audit == null)
            {
                return NotFound();
            }

            return View(audit);
        }

        // GET: Audits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Audits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Audit audit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                _context.Add(audit);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Audit created successfully ";
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                TempData["Error"] = "Error creating Audit " + ex.Message;
                return View(audit);
            }
        }

        // GET: Audits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = await _context.AuditLogs.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }
            return View(audit);
        }

        // POST: Audits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Audit audit)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != audit.Id)
            {
                return NotFound();
            }
            if (!AuditExists(id))
            {
                return NotFound();
            }

            try
                {
                    _context.Update(audit);
                    TempData["Message"] = "Audit updated successfully ";
                    await _context.SaveChangesAsync(userId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                   
                    TempData["Error"] = "Error updated Audit " + ex.Message;

                return View(audit);
            }
                
            
            
        }

        // GET: Audits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var audit = await _context.AuditLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (audit == null)
            {
                return NotFound();
            }

            return View(audit);
        }

        // POST: Audits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var audit = await _context.AuditLogs.FindAsync(id);
            if (audit != null)
            {
                _context.AuditLogs.Remove(audit);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool AuditExists(int id)
        {
            return _context.AuditLogs.Any(e => e.Id == id);
        }
    }
}
