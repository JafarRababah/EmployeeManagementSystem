using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
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
    public class ApprovalEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ApprovalEntries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ApprovalEntries.Include(a => a.Approver).Include(a => a.DocumentType).Include(a => a.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ApprovalEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalEntry = await _context.ApprovalEntries
                .Include(a => a.Approver)
                .Include(a => a.DocumentType)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalEntry == null)
            {
                return NotFound();
            }

            return View(approvalEntry);
        }

        // GET: ApprovalEntries/Create
        public IActionResult Create()
        {
            ViewData["ApproverId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "DocumentTypes"), "Id", "Description");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus"), "Id", "Description");
            return View();
        }

        // POST: ApprovalEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApprovalEntry approvalEntry)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                approvalEntry.LastModifiedId = User.GetUserId();
                approvalEntry.LastModifiedOn = DateTime.Now;
                _context.Add(approvalEntry);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Approve Entry created successfully ";
                return RedirectToAction(nameof(Index));
            }
               catch (Exception ex)
            {
                TempData["Error"] = "Error creating Approve Entry " + ex.Message;
                ViewData["ApproverId"] = new SelectList(_context.Users, "Id", "FullName", approvalEntry.ApproverId);
                ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "DocumentTypes"), "Id", "Description", approvalEntry.DocumentTypeId);
                ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus"), "Id", "Description", approvalEntry.StatusId);
                return View(approvalEntry);
            }
            
           
        }

        // GET: ApprovalEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalEntry = await _context.ApprovalEntries.FindAsync(id);
            if (approvalEntry == null)
            {
                return NotFound();
            }
            ViewData["ApproverId"] = new SelectList(_context.Users, "Id", "FullName", approvalEntry.ApproverId);
            ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "DocumentTypes"), "Id", "Description", approvalEntry.DocumentTypeId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus"), "Id", "Description", approvalEntry.StatusId);
            return View(approvalEntry);
        }

        // POST: ApprovalEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApprovalEntry approvalEntry)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != approvalEntry.Id)
            {
                return NotFound();
            }
            if (!ApprovalEntryExists(approvalEntry.Id))
            {
                return NotFound();
            }

            try
            {
                approvalEntry.LastModifiedId = User.GetUserName();
                approvalEntry.LastModifiedOn = DateTime.UtcNow;
                _context.Update(approvalEntry);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Approve Entry updated successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updated Approve Entry " + ex.Message;
                ViewData["ApproverId"] = new SelectList(_context.Users, "Id", "FullName", approvalEntry.ApproverId);
                ViewData["DocumentTypeId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCodeValue).Where(y=>y.SystemCodeValue.Code== "DocumentTypes"), "Id", "Description", approvalEntry.DocumentTypeId);
                ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCodeValue).Where(y=>y.SystemCodeValue.Code== "LeaveApprovalStatus"), "Id", "Description", approvalEntry.StatusId);
                return View(approvalEntry);
            }                
                
            
            
        }

        // GET: ApprovalEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalEntry = await _context.ApprovalEntries
                .Include(a => a.Approver)
                .Include(a => a.DocumentType)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalEntry == null)
            {
                return NotFound();
            }

            return View(approvalEntry);
        }

        // POST: ApprovalEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var approvalEntry = await _context.ApprovalEntries.FindAsync(id);
            if (approvalEntry != null)
            {
                _context.ApprovalEntries.Remove(approvalEntry);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalEntryExists(int id)
        {
            return _context.ApprovalEntries.Any(e => e.Id == id);
        }
    }
}
