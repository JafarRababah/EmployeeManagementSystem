using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
using EmployeesManagment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Security.Claims;

namespace EmployeesManagment.Controllers
{
    
    public class LeaveBalancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LeaveBalancesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var reasult=await _context.Employees.Include(x => x.Status).ToListAsync();
            return View(reasult);
        }
        [HttpGet]
        public IActionResult AdjustLeaveBalance(int id)
        {
            LeaveAdjustmentEntry leavedAdjustment = new();
            leavedAdjustment.EmployeeId= id;
            ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails
                .Include(y=>y.SystemCodeValue)
                .Where(x=>x.SystemCodeValue.Code=="LeaveAdjustment"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName",id);
            ViewData["LeavePeriodId"] = new SelectList(_context.LeavePeriods.Where(x => x.Closed == false), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustLeaveBalance(LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
            try
            {
                var adjustmentType = _context.SystemCodeDetails
             .Include(x => x.SystemCodeValue)
             .Where(y => y.SystemCodeValue.Code == "LeaveAdjustment" && y.Id == leaveAdjustmentEntry.AdjustmentTypeId).FirstOrDefault();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (ModelState.IsValid)
                {

                    leaveAdjustmentEntry.AdjustmentDescription = leaveAdjustmentEntry.AdjustmentDescription + "-" + adjustmentType.Description;
                    leaveAdjustmentEntry.Id = 0;
                    _context.Add(leaveAdjustmentEntry);
                    await _context.SaveChangesAsync(userId);
                    return RedirectToAction(nameof(Index));
                }
                var employee = await _context.Employees.FindAsync(leaveAdjustmentEntry.EmployeeId);
                employee.LeaveOutStandingBalance = employee.AllocatedLeaveDays - leaveAdjustmentEntry.NoOfDays;
                if (adjustmentType.Code == "Positive")
                {
                    employee.LeaveOutStandingBalance = (employee.AllocatedLeaveDays + leaveAdjustmentEntry.NoOfDays);
                }
                else
                {
                    employee.LeaveOutStandingBalance = (employee.AllocatedLeaveDays - leaveAdjustmentEntry.NoOfDays);
                }
                _context.Update(employee);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Leave Balance created successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) 
            {
                TempData["Error"] = "Error creating Leave Balance " + ex.Message;
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
                ViewData["LeavePeriodId"] = new SelectList(_context.LeavePeriods.Where(x => x.Closed == false), "Id", "Name", leaveAdjustmentEntry.LeavePeriodId);
                return View(leaveAdjustmentEntry);
            }
        }
        // GET: LeaveAdjustmentEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAdjustment = await _context.LeaveAdjustmentEntries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAdjustment == null)
            {
                return NotFound();
            }

            return View(leaveAdjustment);
        }

    }
}
