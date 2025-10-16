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
            var applicationDbContext = _context.Salaries.Include(s => s.Bank).Include(s => s.Employee);
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

        // GET: Salaries/Create
        public IActionResult Create()
        {
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Salary salary)
        {
            var employee = _context.Salaries
                .Where(x => x.EmployeeId == salary.EmployeeId);
            if (employee!=null)
            {
                TempData["Error"] = "this Employee already has salary ";
                ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", salary.BankId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", salary.EmployeeId);
                return View(salary);
            }
            var userId = User.GetUserId();
            try
            {
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
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Id", salary.BankId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", salary.EmployeeId);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    salary.ModifiedOn = DateTime.Now;
                    salary.ModifiedById = User.GetUserName();
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
