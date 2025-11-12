using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Authorization;
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
    public class SalariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salaries
        public async Task<IActionResult> Index()
        {
            var awaitingStatus =  _context.SystemCodeDetails
                 .Include(x => x.SystemCodeValue)
                 .Where(y => y.SystemCodeValue.Code == "SalaryApprovalStatus" && y.Code == "AwaitingApproval").FirstOrDefault();
            var pendingStatus = await _context.SystemCodeDetails
                   .Include(x => x.SystemCodeValue)
                   .FirstOrDefaultAsync(y => y.Code == "Pending" && y.SystemCodeValue.Code == "SalaryApprovalStatus");
            var salaryApplication = _context.Salaries
                .Include(l => l.Employee)
                .Where(l => l.Status == awaitingStatus || l.Status==pendingStatus).OrderByDescending(l => l.CreatedOn);

            return View(salaryApplication);
        }
        public async Task<IActionResult> ApprovedSalaryApplications()
        {
            var approvedStatus = _context.SystemCodeDetails.Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "SalaryApprovalStatus" && y.Code == "Approved").FirstOrDefault();

            var applicationDbContext = _context.Salaries.
                Include(l => l.Employee).
                Include(l => l.Status).
                Where(l => l.StatusId == approvedStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> RejectedSalaryApplications()
        {
            var rejectedStatus = _context.SystemCodeDetails.Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "SalaryApprovalStatus" && y.Code == "Rejected").FirstOrDefault();

            var applicationDbContext = _context.Salaries.
                Include(l => l.Employee).
                Include(l => l.Status).
                Where(l => l.StatusId == rejectedStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Salaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Bank)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApprovedSalary(int? id)
        {
            var salary = await _context.Salaries
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View(salary);

        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApprovedSalary(Salary salary)
        {
            try
            {
                var approvedStatus = _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "SalaryApprovalStatus" && y.Code == "Approved").FirstOrDefault();
                var salaryApplication = await _context.Salaries
                    .Include(l => l.Employee)
                    .Include(l => l.Status)
                    .FirstOrDefaultAsync(m => m.Id == salary.Id);
                if (salaryApplication == null)
                {
                    return NotFound();
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salaryApplication.ApprovedOn = DateTime.Now;
                salaryApplication.ApprovedById = User.GetUserName();
                salaryApplication.StatusId = approvedStatus!.Id;
                salaryApplication.ApprovalNotes = salary.ApprovalNotes;
                _context.Update(salaryApplication);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Salary application approved successfully ";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Salary application Not approved by successfully ";
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                return View(salary);
            }

        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectSalary(int? id)
        {
            var salary = await _context.Salaries
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View(salary);

        }
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectSalary(Salary salary)
        {
            try
            {
                var rejectedStatus = _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "SalaryApprovalStatus" && y.Code == "Rejected").FirstOrDefault();
                var salaryApplication = await _context.Salaries
                    .Include(l => l.Employee)
                    .Include(l => l.Status)
                    .FirstOrDefaultAsync(m => m.Id == salary.Id);
                if (salaryApplication == null)
                {
                    return NotFound();
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                salaryApplication.ApprovedOn = DateTime.Now;
                salaryApplication.ApprovedById = User.GetUserName();
                salaryApplication.StatusId = rejectedStatus!.Id;
                salaryApplication.ApprovalNotes = salary.ApprovalNotes;
                _context.Update(salaryApplication);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Salary application Rejected successfully ";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Salary application Not Rejected by successfully ";
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                return View(salary);
            }

        }
        // GET: Salaries/Create
        public async Task<IActionResult> Create(int? employeeId)
        {
            var employees = await _context.Employees.ToListAsync();
            ViewData["EmployeeId"] = new SelectList(employees, "Id", "FullName", employeeId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            if (employeeId != null)
            {
                var employee = employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee == null)
                    return NotFound();
                ViewBag.EmployeeName = employee.FullName;

            }
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary salary)
        {

            var hasSalary =await  _context.Salaries
                .AnyAsync(x => x.EmployeeId == salary.EmployeeId);
            if (hasSalary)
            {
                TempData["Error"] = "this Employee already has salary ";
                ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", salary.BankId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
                return View(salary);
            }
            var userId = User.GetUserId();
            var pendingStatus = await _context.SystemCodeDetails
                   .Include(x => x.SystemCodeValue)
                   .FirstOrDefaultAsync(y => y.Code == "AwaitingApproval" && y.SystemCodeValue.Code == "SalaryApprovalStatus");
            if (pendingStatus == null)
            {
                ModelState.AddModelError("", "Status 'AwaitingApproval' not found.");
                return View(salary);
            }
            try
            {
                salary.StatusId=pendingStatus.Id;
                salary.CreatedById = User.GetUserName();
                salary.CreatedOn = DateTime.Now;
                salary.ModifiedById = User.GetUserName();
                salary.ModifiedOn = DateTime.Now;
                salary.ApprovedById = User.GetUserName();
                salary.ApprovedOn = DateTime.Now;
                _context.Add(salary);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Salary created successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Salary Not created by successfully ";
                ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", salary.BankId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
                return View(salary);
            }
        }

        // GET: Salaries/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", salary.BankId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Salary salary)
        {
            var userId = User.GetUserId();
            if (id != salary.Id)
            {
                return NotFound();
            }
            if (!SalaryExists(salary.Id))
            {
                return NotFound();
            }

            try
                {
                var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.Code == "Pending" && y.SystemCodeValue.Code == "SalaryApprovalStatus")
                .FirstOrDefaultAsync();
                salary.ModifiedOn = DateTime.Now;
                salary.ModifiedById = User.GetUserName();
                salary.StatusId = pendingStatus.Id;
                _context.Update(salary);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
                {
                TempData["Error"] = "Salary Not Updated by successfully ";
                ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", salary.BankId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
                return View(salary);
            }
            
           
        }

        // GET: Salaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Bank)
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary != null)
            {
                _context.Salaries.Remove(salary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.Id == id);
        }
    }
}
