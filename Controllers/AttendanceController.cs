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
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Employees.Include(e => e.Bank).Include(e => e.CauseOfInactivity).Include(e => e.Country).Include(e => e.Department).Include(e => e.Designation).Include(e => e.Disability).Include(e => e.EmploymentTerms).Include(e => e.Gender).Include(e => e.ReasonForTermination).Include(e => e.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.CauseOfInactivity)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Disability)
                .Include(e => e.EmploymentTerms)
                .Include(e => e.Gender)
                .Include(e => e.ReasonForTermination)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id");
            ViewData["CauseOfInactivityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Id");
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            ViewData["ReasonForTerminationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id");
            return View();
        }

        // POST: Attendance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmpNo,FirstName,MiddleName,LastName,PhoneNumber,EmailAddress,CountryId,DateOfBirth,Address,DepartmentId,DesignationId,GenderId,Photo,EmploymentDate,StatusId,InactiveDate,CauseOfInactivityId,TerminationDate,ReasonForTerminationId,BankId,BankAccountNo,IBAN,SWIFTCode,NSSFNO,NHIF,CompanyEmail,KRAPIN,PassportNo,EmploymentTermsId,AllocatedLeaveDays,LeaveOutStandingBalance,PaysTax,DisabilityId,DisabilityCertificate,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", employee.BankId);
            ViewData["CauseOfInactivityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.CauseOfInactivityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Id", employee.DesignationId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.DisabilityId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.EmploymentTermsId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.GenderId);
            ViewData["ReasonForTerminationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.ReasonForTerminationId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.StatusId);
            return View(employee);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", employee.BankId);
            ViewData["CauseOfInactivityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.CauseOfInactivityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Id", employee.DesignationId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.DisabilityId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.EmploymentTermsId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.GenderId);
            ViewData["ReasonForTerminationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.ReasonForTerminationId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.StatusId);
            return View(employee);
        }

        // POST: Attendance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpNo,FirstName,MiddleName,LastName,PhoneNumber,EmailAddress,CountryId,DateOfBirth,Address,DepartmentId,DesignationId,GenderId,Photo,EmploymentDate,StatusId,InactiveDate,CauseOfInactivityId,TerminationDate,ReasonForTerminationId,BankId,BankAccountNo,IBAN,SWIFTCode,NSSFNO,NHIF,CompanyEmail,KRAPIN,PassportNo,EmploymentTermsId,AllocatedLeaveDays,LeaveOutStandingBalance,PaysTax,DisabilityId,DisabilityCertificate,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", employee.BankId);
            ViewData["CauseOfInactivityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.CauseOfInactivityId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", employee.CountryId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Id", employee.DesignationId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.DisabilityId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.EmploymentTermsId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.GenderId);
            ViewData["ReasonForTerminationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.ReasonForTerminationId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", employee.StatusId);
            return View(employee);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Bank)
                .Include(e => e.CauseOfInactivity)
                .Include(e => e.Country)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Disability)
                .Include(e => e.EmploymentTerms)
                .Include(e => e.Gender)
                .Include(e => e.ReasonForTermination)
                .Include(e => e.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
