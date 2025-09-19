using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Models;

namespace EmployeesManagment.Controllers
{
    public class LeaveAdjustmentEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveAdjustmentEntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LeaveAdjustmentEntries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LeaveAdjustmentEntries.Include(l => l.AdjustmentType).Include(l => l.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LeaveAdjustmentEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAdjustmentEntry = await _context.LeaveAdjustmentEntries
                .Include(l => l.AdjustmentType)
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAdjustmentEntry == null)
            {
                return NotFound();
            }

            return View(leaveAdjustmentEntry);
        }

        // GET: LeaveAdjustmentEntries/Create
        public IActionResult Create()
        {
            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: LeaveAdjustmentEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
            try
            {
                
                    _context.Add(leaveAdjustmentEntry);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Leave Adjustment created successfully ";
                    return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating Leave Adjustment " + ex.Message;
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
                return View(leaveAdjustmentEntry);
            }
           
        }

        // GET: LeaveAdjustmentEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAdjustmentEntry = await _context.LeaveAdjustmentEntries.FindAsync(id);
            if (leaveAdjustmentEntry == null)
            {
                return NotFound();
            }
            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
            return View(leaveAdjustmentEntry);
        }

        // POST: LeaveAdjustmentEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
            if (id != leaveAdjustmentEntry.Id)
            {
                return NotFound();
            }
            if (!LeaveAdjustmentEntryExists(leaveAdjustmentEntry.Id))
            {
                return NotFound();
            }

            try
            {
                    _context.Update(leaveAdjustmentEntry);
                    await _context.SaveChangesAsync();
                TempData["Message"] = "Leave Adjustment updated successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
                {
                TempData["Error"] = "Error updated Leave Adjustment " + ex.Message;
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
                return View(leaveAdjustmentEntry);
            }
            
           
        }

        // GET: LeaveAdjustmentEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAdjustmentEntry = await _context.LeaveAdjustmentEntries
                .Include(l => l.AdjustmentType)
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAdjustmentEntry == null)
            {
                return NotFound();
            }

            return View(leaveAdjustmentEntry);
        }

        // POST: LeaveAdjustmentEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveAdjustmentEntry = await _context.LeaveAdjustmentEntries.FindAsync(id);
            if (leaveAdjustmentEntry != null)
            {
                _context.LeaveAdjustmentEntries.Remove(leaveAdjustmentEntry);
            }
            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "LeaveAdjust Entry deleted successfully ";
                return RedirectToAction(nameof(Index));
            }
           
             catch (Exception ex)
            {
                TempData["Error"] = "Error delete leaveAdjust Entry " + ex.Message;
                return View(leaveAdjustmentEntry);
            }
        }

        private bool LeaveAdjustmentEntryExists(int id)
        {
            return _context.LeaveAdjustmentEntries.Any(e => e.Id == id);
        }
    }
}
