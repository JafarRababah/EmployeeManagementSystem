using ClosedXML.Excel;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            payroll.CreatedById = User.GetUserName();
            payroll.CreatedOn = DateTime.Now;
            payroll.ModifiedById = User.GetUserName();
            payroll.ModifiedOn = DateTime.Now;
            
            bool isOverlap = await _context.Payrolls
                   .AnyAsync(p => payroll.PeriodStart <= p.PeriodEnd && payroll.PeriodEnd >= p.PeriodStart);
            if (isOverlap)
            {
                TempData["Error"] = "This Period already Exist. please choose other period";
                return View(payroll);
            }
          
            var attendances = _context.Attendances
                     .Where(a => a.EmployeeId == payroll.EmployeeId &&
                     a.Date >= payroll.PeriodStart &&
                     a.Date <= payroll.PeriodEnd)
                     .ToList();
            if (!attendances.Any())
            {
                TempData["Error"] = "No attendance records found for the selected period.";
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", payroll.EmployeeId);
                return View(payroll);
            }
            decimal overtimePay = 0;
            decimal latePenalty = 0;
            int totalDays = (payroll.PeriodEnd - payroll.PeriodStart).Days + 1;
            int presentDays = attendances.Count(a => a.StatusId == 1); // „À·« 1 = Õ«÷—
            int absentDays = totalDays - presentDays;
            decimal dailySalary = payroll.BasicSalary / totalDays;
            decimal absenceDeduction = absentDays * dailySalary;
            foreach (var att in attendances)
            {
                // ﬂ· ”«⁄… ≈÷«›Ì… = √Ã— «·”«⁄… «·√”«”Ì… ◊ 1.5 „À·«
                var hourlyRate = payroll.BasicSalary / 30 / 8; // ÌÊ„ = 8 ”«⁄« 
                overtimePay += (decimal)att.OvertimeHours * hourlyRate * 1.5m;

                // ﬂ· œﬁÌﬁ…  √ŒÌ—  Œ’„ ‰”»… „‰ «·√Ã— «·ÌÊ„Ì
                latePenalty += (decimal)att.LateMinutes * (hourlyRate / 60);
            }
            payroll.Overtime = overtimePay;
            payroll.Penalty = latePenalty;
            
            if (payroll.IsFullAttendance)
            {
                
                    // «·„ÊŸ› ·œÌÂ Õ÷Ê— ﬂ«„·
                    payroll.Bonus += payroll.BasicSalary * 0.05m; // „ﬂ«›√… 5%
                    TempData["Message"] = $"Payroll created with full attendance bonus for {payroll.PeriodStart:MMMM yyyy}.";
                
                
            }
            payroll.NetSalary = (payroll.BasicSalary + payroll.Allowances + payroll.Overtime + payroll.Bonus) -
               (payroll.Deductions + payroll.Tax + payroll.Penalty + payroll.SocialSecurity + absenceDeduction);
            try
            {
                _context.Add(payroll);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = $"Payroll created successfully for {payroll.PeriodStart:MMMM yyyy}";
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
        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
        public IActionResult PayrollReportPdf(int employeeId)
        {
            var payrolls = _context.Payrolls
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.PeriodStart)
                .ToList();

            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (employee == null || !payrolls.Any())
                return NotFound();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"ﬂ‘› —Ê« » «·„ÊŸ›: {employee.FullName}").FontSize(20).Bold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100);
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("«·› —… „‰");
                            header.Cell().Text("≈·Ï");
                            header.Cell().Text("«·—« » «·√”«”Ì");
                            header.Cell().Text("«·»œ·« ");
                            header.Cell().Text("«·’«›Ì");
                        });

                        // Rows
                        foreach (var p in payrolls)
                        {
                            table.Cell().Text(p.PeriodStart.ToShortDateString());
                            table.Cell().Text(p.PeriodEnd.ToShortDateString());
                            table.Cell().Text($"{p.BasicSalary:C}");
                            table.Cell().Text($"{p.Allowances:C}");
                            table.Cell().Text($"{p.NetSalary:C}");
                        }
                    });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Payroll_{employee.FullName}.pdf");
        }
        [Authorize(Roles = "Manager")]
        public IActionResult PayrollReportExcel(int employeeId)
        {
            var payrolls = _context.Payrolls
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.PeriodStart)
                .ToList();

            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (employee == null || !payrolls.Any())
                return NotFound();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Payroll Report");

            worksheet.Cell(1, 1).Value = "«·› —… „‰";
            worksheet.Cell(1, 2).Value = "≈·Ï";
            worksheet.Cell(1, 3).Value = "«·—« » «·√”«”Ì";
            worksheet.Cell(1, 4).Value = "«·»œ·« ";
            worksheet.Cell(1, 5).Value = "«·’«›Ì";

            int row = 2;
            foreach (var p in payrolls)
            {
                worksheet.Cell(row, 1).Value = p.PeriodStart.ToShortDateString();
                worksheet.Cell(row, 2).Value = p.PeriodEnd.ToShortDateString();
                worksheet.Cell(row, 3).Value = p.BasicSalary;
                worksheet.Cell(row, 4).Value = p.Allowances;
                worksheet.Cell(row, 5).Value = p.NetSalary;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Payroll_{employee.FullName}.xlsx");
        }
        public IActionResult PayrollReport(DateTime? startDate, DateTime? endDate)
        {
            var payrolls = _context.Payrolls.Include(p => p.Employee).AsQueryable();

            if (startDate.HasValue)
                payrolls = payrolls.Where(p => p.PeriodStart >= startDate.Value);

            if (endDate.HasValue)
                payrolls = payrolls.Where(p => p.PeriodEnd <= endDate.Value);

            var model = payrolls.OrderByDescending(p => p.PeriodStart).ToList();
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(model);
        }

    }
}
