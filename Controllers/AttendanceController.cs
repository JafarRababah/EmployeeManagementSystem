using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
        [HttpGet]
        public IActionResult Import()
        {
            return View(new AttendanceImportViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Import(AttendanceImportViewModel model)
        {
            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                ModelState.AddModelError("", "«·—Ã«¡ «Œ Ì«— „·› CSV ’«·Õ.");
                return View(model);
            }

            using var reader = new StreamReader(model.CsvFile.OpenReadStream());
            string? line;
            int count = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.StartsWith("EmployeeId")) continue; //  Ã«Ê“ «·⁄‰Ê«‰

                var columns = line.Split(',');

                if (columns.Length < 3) continue;

                if (!int.TryParse(columns[0], out var empId))
                    continue;

                if (!DateTime.TryParseExact(columns[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var checkIn))
                    continue;

                DateTime? checkOut = null;
                if (DateTime.TryParseExact(columns[2], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var tempOut))
                    checkOut = tempOut;

                var attendance = new Attendance
                {
                    EmployeeId = empId,
                    Date = checkIn.Date,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    OvertimeHours = int.Parse(columns[4]),
                    LateMinutes = int.Parse(columns[5]),
                    CreatedOn = columns.Length > 6 ? DateTime.Parse(columns[6]) : DateTime.Now,
                    ModifiedOn = columns.Length > 7 ? DateTime.Parse(columns[7]) : DateTime.Now,
                    StatusId = int.Parse(columns[8]),
                    Source = "Fingerprint"
                };


                await _context.Attendances.AddAsync(attendance);
                count++;
            }
            var userId = User.GetUserId();
            await _context.SaveChangesAsync(userId);
            ViewBag.Message = $" „ «” Ì—«œ {count} ”Ã· Õ÷Ê— »‰Ã«Õ!";
            return View(new AttendanceImportViewModel());
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
                attendance.CreatedById =userId;
                attendance.ModifiedById =userId;
                attendance.CreatedOn = DateTime.Now;
                attendance.ModifiedOn = DateTime.Now;
                // Õ”«» «·Õ«·… »‘ﬂ·  ·ﬁ«∆Ì
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
            var userId = User.GetUserId();
            if (id != attendance.Id) return NotFound();
            if (!_context.Attendances.Any(e => e.Id == attendance.Id))
                return NotFound();

            try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Attendance created successfully ";
                return RedirectToAction(nameof(Index));
            }
                catch (Exception ex)
                {
                TempData["Error"] = "Attendance Not updated by successfully ";
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", attendance.EmployeeId);
                return View(attendance);
            }
                
            

            
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
