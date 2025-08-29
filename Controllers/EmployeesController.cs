using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using System.Security.Claims;
using EmployeesManagment.ViewModels;
using AutoMapper;
using EmployeesManagment.Services;

namespace EmployeesManagment.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public EmployeesController(IConfiguration configuration, ApplicationDbContext context,IMapper mapper, IExtensionService extesionService)
        {
            _context = context;
            _configuration= configuration;
            _mapper=mapper;
            _extesionService = extesionService;
        }

        // GET: Employees
        public async Task<IActionResult> Index(EmployeeViewModel employees)
        {
            var rowdata =  _context.Employees.Include(x => x.Status).AsQueryable();
            if (!string.IsNullOrEmpty(employees.FullName.Trim()))
            {

                rowdata = rowdata.Where(x => x.FullName.Contains(employees.FullName));
            }
            if (employees.PhoneNumber>0)
            {
                rowdata = rowdata.Where(x => x.PhoneNumber==employees.PhoneNumber);
            }
            if (!string.IsNullOrEmpty(employees.EmpNo))
            {
                rowdata = rowdata.Where(x => x.EmpNo.Contains(employees.EmpNo));
            }
            employees.Employees = await _context.Employees
                .Include(x=>x.Status)
                .AsQueryable()
                .Where(x=>x.Id>0).OrderByDescending(x=>x.EmpNo).ToListAsync();
            return View(employees);
            //return View(await _context.Employees.Include(x=>x.Status).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(x=>x.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name");
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "EmploymentTerms"), "Id", "Description");
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "DisabilityTypes"), "Id", "Description");
            ViewData["GenderId"] =          new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x=>x.SystemCodeValue.Code=="Gender"), "Id", "Description");
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel newemployee,IFormFile employeephoto)
        {
            var employee = new Employee();
            try
            {
                
                _mapper.Map(newemployee, employee);
                employee.EmpNo = await _extesionService.GenerateEmployeeNumber();
                if (employeephoto.Length > 0)
                {
                    var fileName = "EmployeePhoto_" + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + employeephoto.FileName;
                    var path = _configuration["FileSettings:UploadFolder"];
                    var filepath = Path.Combine(path, fileName);
                    var stream = new FileStream(filepath, FileMode.Create);
                    await employeephoto.CopyToAsync(stream);
                    employee.Photo = fileName;
                }
                var statusId = await _context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "EmployeeStatus" && x.Code == "Active").FirstOrDefaultAsync();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                employee.CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
                employee.CreatedOn = DateTime.Now;
                employee.StatusId = statusId.Id;
                if (!ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Employee Created Successfuly";
                    return RedirectToAction(nameof(Index));
                }

            } 


                
                

            
            catch (Exception ex)
            {
                TempData["Error"] = "Employee could be created Successfuly " + ex.Message;
                return View(employee);
            }
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "DisabilityTypes"), "Id", "Description", employee.DisabilityId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "EmploymentTerms"), "Id", "Description", employee.EmploymentTermsId);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
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
            var newEmployee = new EmployeeViewModel();
            _mapper.Map(employee, newEmployee);
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "EmploymentTerms"), "Id", "Description", employee.EmploymentTermsId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "DisabilityTypes"), "Id", "Description", employee.DisabilityId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Employee employee)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != employee.Id)
            {
                return NotFound();
            }
            //ModelState.Remove("Bank");
            //ModelState.Remove("Country");
            //ModelState.Remove("Department");
            //ModelState.Remove("Designation");
            //ModelState.Remove("ApplicationUser");
            //ModelState.Remove("CreateBy");
            //ModelState.Remove("ModifiedBy");
            //ModelState.Remove("Employee");
            //ModelState.Remove("Gender");
            //ModelState.Remove("Disability");
            //ModelState.Remove("Status");
            //ModelState.Remove("CauseOfInactivey");
            //ModelState.Remove("CauseOfInactivey");
            //ModelState.Remove("ReasonForTermination");
            //ModelState.Remove("EmploymentTerms");
            if (!ModelState.IsValid)
            {
                try
                {
                    employee.ModifiedById = User.Identity.Name;
                    employee.ModifiedOn = DateTime.Now;
                    _context.Update(employee);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Employee details updated Successfuly ";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                    TempData["Error"] = "Employee details updated Successfuly " + ex.Message;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "Gender"), "Id", "Description", employee.GenderId);
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", employee.CountryId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["BankId"] = new SelectList(_context.Banks, "Id", "Name", employee.BankId);
            ViewData["EmploymentTermsId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "EmploymentTerms"), "Id", "Description",employee.EmploymentTermsId);
            ViewData["DisabilityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "DisabilityTypes"), "Id", "Description",employee.DisabilityId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
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
