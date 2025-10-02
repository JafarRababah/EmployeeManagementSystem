using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;

namespace EmployeesManagment.Controllers
{
    public class PayrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Payroll
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Payrolls.Include(p => p.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Payroll/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }
  
        // GET: Payroll/Create
        public IActionResult Create(int? employeeId)
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            if (employeeId != null)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee == null) return NotFound();

                ViewBag.EmployeeId = employee.Id;
                ViewBag.EmployeeName = employee.FullName;
            }
            return View();
        }

        // POST: Payroll/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payroll payroll)
        {
            var userId = User.GetUserId();
            payroll.NetSalary = (payroll.BasicSalary + payroll.Allowances + payroll.Overtime) - (payroll.Deductions);
            
            try
            {
                _context.Add(payroll);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Payroll created successfully ";
                return RedirectToAction("Details", "Employees", new { id = payroll.EmployeeId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating payroll " + ex.Message;
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", payroll.EmployeeId);
                return View(payroll);
            }
        }

        // GET: Payroll/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", payroll.EmployeeId);
            return View(payroll);
        }

        // POST: Payroll/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payroll payroll)
        {
            var userId = User.GetUserId();
            if (id != payroll.PayrollId)
            {
                return NotFound();
            }
            if (!PayrollExists(payroll.PayrollId))
            {
                return NotFound();
            }
           
                try
                {
                    _context.Update(payroll);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Payroll updated successfully ";
                return RedirectToAction(nameof(Index));
            }
                catch (Exception ex)
                {
                    
                    TempData["Error"] = "Error updated payroll " + ex.Message;
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", payroll.EmployeeId);
                return View(payroll);
            }
              
            
            
        }

        // GET: Payroll/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayrollId == id);
            if (payroll == null)
            {
                return NotFound();
            }

            return View(payroll);
        }

        // POST: Payroll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll != null)
            {
                _context.Payrolls.Remove(payroll);
            }
            try
            {
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Payroll deleted successfully ";
                return RedirectToAction(nameof(Index));
            }
           
             catch (Exception ex)
            {
                TempData["Error"] = "Error delete payroll " + ex.Message;
                return View(payroll);
            }
        }

        private bool PayrollExists(long id)
        {
            return _context.Payrolls.Any(e => e.PayrollId == id);
        }
    }
}
