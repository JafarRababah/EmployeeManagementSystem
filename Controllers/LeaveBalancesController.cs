using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
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
            leavedAdjustment.EmployeeId = id;

            ViewData["AdjustmentTypeId"] = new SelectList(
                _context.SystemCodeDetails
                    .Include(y => y.SystemCodeValue)
                    .Where(x => x.SystemCodeValue.Code == "LeaveAdjustment"),
                "Id",
                "Description"
            );

            ViewData["EmployeeId"] = new SelectList(
                _context.Employees,
                "Id",
                "FullName",
                id // هذا يحدد الـSelectedValue
            );

            ViewData["LeavePeriodId"] = new SelectList(
                _context.LeavePeriods.Where(x => !x.Closed),
                "Id",
                "Name"
            );

            // ترجع الموديل للـView
            return View(leavedAdjustment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustLeaveBalance(LeaveAdjustmentEntry leaveAdjustmentEntry)
        {
            try
            {
                var adjustmentType = await _context.SystemCodeDetails
                    .Include(x => x.SystemCodeValue)
                    .FirstOrDefaultAsync(y =>
                        y.SystemCodeValue.Code == "LeaveAdjustment"
                        && y.Id == leaveAdjustmentEntry.AdjustmentTypeId);

                if (adjustmentType == null)
                {
                    TempData["Error"] = "Invalid adjustment type.";
                    return View(leaveAdjustmentEntry);
                }
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                leaveAdjustmentEntry.ModifiedById = User.GetUserName(); ;
                leaveAdjustmentEntry.ModifiedOn = DateTime.Now;

                

                // إعداد الوصف
                leaveAdjustmentEntry.AdjustmentDescription =
                    (leaveAdjustmentEntry.AdjustmentDescription ?? "") +
                    $" - {adjustmentType.Description}";
                leaveAdjustmentEntry.Id = 0; // للتأكد من أنه إدخال جديد

                // جلب الموظف
                var employee = await _context.Employees.FindAsync(leaveAdjustmentEntry.EmployeeId);
                if (employee == null)
                {
                    TempData["Error"] = "Employee not found.";
                    return RedirectToAction(nameof(Index));
                }

                // حساب الرصيد الجديد
                if (adjustmentType.Code == "Positive")
                {
                    employee.LeaveOutStandingBalance += leaveAdjustmentEntry.NoOfDays;
                }
                else
                {
                    employee.LeaveOutStandingBalance -= leaveAdjustmentEntry.NoOfDays;
                }
                
                // حفظ التعديل وسجل الحركة معاً
                _context.LeaveAdjustmentEntries.Add(leaveAdjustmentEntry);
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync(userId);

                TempData["Message"] = $"Leave balance adjusted successfully for {employee.FullName}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating Leave Balance: " + ex.Message;
                ViewData["AdjustmentTypeId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveAdjustmentEntry.AdjustmentTypeId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveAdjustmentEntry.EmployeeId);
                ViewData["LeavePeriodId"] = new SelectList(_context.LeavePeriods.Where(x => !x.Closed), "Id", "Name", leaveAdjustmentEntry.LeavePeriodId);
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
