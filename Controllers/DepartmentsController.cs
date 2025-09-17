using AutoMapper;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public DepartmentsController(ApplicationDbContext context, IMapper mapper, IExtensionService extensionService)
        {
            _context = context;
            _mapper = mapper;
            _extesionService = extensionService;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var departments=await _context.Departments.ToListAsync();
            return View(departments);
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                department.Code = await _extesionService.GenerateDepartmentNumber();
                var userName = User.Identity.Name;
                    department.CreatedById = userName;
                    department.ModifiedById = userName;
                    department.CreatedOn = DateTime.Now;
                    department.ModifiedOn = DateTime.Now;
                    _context.Add(department);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Department created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating department " + ex.Message;
                return View(department);
            }
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Department department)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != department.Id)
            {
                return NotFound();
            }
            if (!DepartmentExists(department.Id))
            {
                return NotFound();
            }

            
                try
                {
                    department.ModifiedById = User.Identity.Name;
                    department.ModifiedOn = DateTime.Now;
                    _context.Update(department);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Department updated successfully ";
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error creating department " + ex.Message;

                    return View(department);

                }
            
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            try
            {
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Department deleted successfully ";
                return RedirectToAction(nameof(Index));
            }
           
            catch (Exception ex)
            {
                TempData["Error"] = "Error delete department " + ex.Message;
                return View(department);
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
