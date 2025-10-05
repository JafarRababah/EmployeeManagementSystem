using System;
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
            var data = _context.Attendances.Include(a => a.Employee);
            return View(await data.ToListAsync());
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            return View();
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            var userId = User.GetUserId();
            try
            {
                attendance.Date = DateTime.Now;
                attendance.CreatedById =userId;
                attendance.ModifiedById =userId;
                attendance.CreatedOn = DateTime.Now;
                attendance.ModifiedOn = DateTime.Now;
                // ÍÓÇÈ ÇáÍÇáÉ ÈÔßá ÊáÞÇÆí
                if (attendance.CheckIn.HasValue)
                {
                    if (attendance.CheckIn.Value.TimeOfDay > new TimeSpan(9, 0, 0))
                        attendance.StatusId = 5016;
                    else
                        attendance.StatusId = 5015;
                }
                else
                {
                    attendance.StatusId =5017;
                }

                _context.Add(attendance);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Attendance created successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Attendance Not created by successfully ";
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
                return View(attendance);
            }
          
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return NotFound();

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
            return View(attendance);
        }

        // POST: Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            if (id != attendance.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attendances.Any(e => e.Id == attendance.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
            return View(attendance);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attendance == null) return NotFound();

            return View(attendance);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
                _context.Attendances.Remove(attendance);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
