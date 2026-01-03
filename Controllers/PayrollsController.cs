using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.ViewModels;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// iText7 aliases to avoid ambiguity with OpenXml types
using ITextParagraph = iText.Layout.Element.Paragraph;
using ITextCell = iText.Layout.Element.Cell;
using ITextTable = iText.Layout.Element.Table;
using ITextDocument = iText.Layout.Document;


namespace EmployeesManagment.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public PayrollsController(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        public async Task<IActionResult> Create(int? employeeId)
        {
            var employees = await _context.Employees.ToListAsync();
            ViewData["EmployeeId"] = new SelectList(employees, "Id", "FullName", employeeId);
            var payroll = new Payroll();
            if (employeeId != null)
            {
                var employee = employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee == null)
                    return NotFound();
                payroll.EmployeeId = employee.Id;
                ViewBag.EmployeeName = employee.FullName;
                
            }

            return View();
        }

        [HttpGet]
        public JsonResult GetSalaryByEmployeeId(int employeeId)
        {
            var salary = _context.Salaries
                .Where(s => s.EmployeeId == employeeId)
                .Select(s => new
                {
                    basicSalary = s.BasicSalary,
                    allowance = s.Allowances,
                    deductions = s.Deductions,
                    netSalary = s.NetSalary
                })
                .FirstOrDefault();

            return Json(salary);
        }
        public IActionResult PeriodReport(DateTime? startDate, DateTime? endDate, int? employeeId)
        {
            // تعبئة قائمة الموظفين في الـ Dropdown
            ViewBag.Employees = new SelectList(_context.Employees.ToList(), "Id", "FullName");

            // تعيين التواريخ الافتراضية إذا لم يختار المستخدم
            startDate ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            endDate ??= startDate.Value.AddMonths(1).AddDays(-1);

            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");

            // جلب البيانات من جدول Payroll
            var query = _context.Payrolls
                .Include(p => p.Employee)
                .Where(p => p.PeriodStart >= startDate && p.PeriodEnd <= endDate);

            // ✅ إذا تم اختيار موظف معيّن، نفلتر عليه
            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(p => p.EmployeeId == employeeId.Value);
            }

            var payrolls = query.ToList();

            // إجمالي الرواتب
            ViewBag.TotalNetSalary = payrolls.Sum(p => p.NetSalary);

            return View(payrolls);
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
                ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", payroll?.EmployeeId);
                return View(payroll);
            }

            var attendances =await _context.Attendances
                     .Where(a => a.EmployeeId == payroll.EmployeeId &&
                     a.Date >= payroll.PeriodStart &&
                     a.Date <= payroll.PeriodEnd)
                     .ToListAsync();
            if (!attendances.Any() && !payroll.IsFullAttendance)
            {
                TempData["Error"] = "No attendance records found for the selected period.";
                ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", payroll?.EmployeeId);
                return View(payroll);
            }
            decimal overtimePay = 0;
            decimal latePenalty = 0;
            int totalDays = (payroll.PeriodEnd - payroll.PeriodStart).Days + 1;
            int presentDays = attendances.Count(a => a.StatusId == 1); // مثلاً 1 = حاضر
            int absentDays = totalDays - presentDays;
            decimal dailySalary = payroll.BasicSalary / totalDays;
            decimal absenceDeduction = absentDays * dailySalary;
            foreach (var att in attendances)
            {
                // كل ساعة إضافية = أجر الساعة الأساسية × 1.5 مثلاً
                var hourlyRate = payroll.BasicSalary / 30 / 8; // يوم = 8 ساعات
                overtimePay += (decimal)att.OvertimeHours * hourlyRate * 1.5m;

                // كل دقيقة تأخير تخصم نسبة من الأجر اليومي
                latePenalty += (decimal)att.LateMinutes * (hourlyRate / 60);
            }
            payroll.Overtime = overtimePay;
            payroll.Penalty = latePenalty;
            
            if (payroll.IsFullAttendance)
            {
                
                    // الموظف لديه حضور كامل
                    payroll.Bonus += payroll.BasicSalary;
                    TempData["Message"] = $"Payroll created with full attendance bonus for {payroll.PeriodStart:MMMM yyyy}.";
                
                
            }
            payroll.NetSalary = (payroll.BasicSalary + payroll.Allowances + payroll.Overtime + payroll.Bonus) -
               (payroll.Deductions + payroll.Tax + payroll.Penalty + payroll.SocialSecurity + absenceDeduction);
            try
            {
                _context.Add(payroll);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = $"Payroll created successfully for {payroll.PeriodStart:MMMM yyyy}";
                return RedirectToAction("Details", "Payrolls", new { id = payroll?.PayrollId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating payroll: " + ex.Message;
                ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", payroll?.EmployeeId);
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
            ViewData["EmployeeId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", payroll?.EmployeeId);
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

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"كشف رواتب الموظف: {employee.FullName}")
                        .FontSize(20)
                        .Bold()
                        .AlignCenter();
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

                        table.Header(header =>
                        {
                            header.Cell().Text("من");
                            header.Cell().Text("إلى");
                            header.Cell().Text("الراتب الأساسي");
                            header.Cell().Text("البدلات");
                            header.Cell().Text("الصافي");
                        });

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

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream.ToArray(), "application/pdf", $"Payroll_{employee.FullName}.pdf");
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

            worksheet.Cell(1, 1).Value = "الفترة من";
            worksheet.Cell(1, 2).Value = "إلى";
            worksheet.Cell(1, 3).Value = "الراتب الأساسي";
            worksheet.Cell(1, 4).Value = "البدلات";
            worksheet.Cell(1, 5).Value = "الصافي";

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
        public IActionResult PayrollReport(DateTime? startDate, DateTime? endDate, int? employeeId)
        {
            var payrolls = _context.Payrolls
                .Include(p => p.Employee)
                .AsQueryable();

            if (startDate.HasValue)
                payrolls = payrolls.Where(p => p.PeriodStart >= startDate.Value);

            if (endDate.HasValue)
                payrolls = payrolls.Where(p => p.PeriodEnd <= endDate.Value);

            if (employeeId.HasValue && employeeId > 0)
                payrolls = payrolls.Where(p => p.EmployeeId == employeeId.Value);

            var model = payrolls.OrderByDescending(p => p.PeriodStart).ToList();

            ViewBag.Employees = new SelectList(_context.Employees, "Id", "FullName");
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.EmployeeId = employeeId;
            ViewBag.TotalNetSalary = model.Sum(p => p.NetSalary);

            return View(model);
        }


        public IActionResult SalaryExportToExcel(DateTime startDate, DateTime endDate)
        {
            var payrolls = _context.Payrolls
                .Where(p => p.PeriodStart >= startDate && p.PeriodEnd <= endDate)
                .Include(p => p.Employee)
                .ToList();

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Payroll Report");

                // العناوين
                ws.Cells["A1"].Value = "Employee";
                ws.Cells["B1"].Value = "Period Start";
                ws.Cells["C1"].Value = "Period End";
                ws.Cells["D1"].Value = "Basic Salary";
                ws.Cells["E1"].Value = "Allowances";
                ws.Cells["F1"].Value = "Deductions";
                ws.Cells["G1"].Value = "Tax";
                ws.Cells["H1"].Value = "Net Salary";

                // تنسيق الرأس
                using (var range = ws.Cells["A1:H1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                int row = 2;
                foreach (var item in payrolls)
                {
                    ws.Cells[row, 1].Value = item.Employee.FullName;
                    ws.Cells[row, 2].Value = item.PeriodStart.ToString("yyyy-MM-dd");
                    ws.Cells[row, 3].Value = item.PeriodEnd.ToString("yyyy-MM-dd");
                    ws.Cells[row, 4].Value = item.BasicSalary;
                    ws.Cells[row, 5].Value = item.Allowances;
                    ws.Cells[row, 6].Value = item.Deductions;
                    ws.Cells[row, 7].Value = item.Tax;
                    ws.Cells[row, 8].Value = item.NetSalary;
                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream(package.GetAsByteArray());
                string fileName = $"PayrollReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        public IActionResult SalaryExportToPdf(DateTime startDate, DateTime endDate)
        {
            var payrolls = _context.Payrolls
                .Where(p => p.PeriodStart >= startDate && p.PeriodEnd <= endDate)
                .Include(p => p.Employee)
                .ToList();

            using (var stream = new MemoryStream())
            {
                using (var writer = new PdfWriter(stream))
                {
                    var pdf = new PdfDocument(writer);
                    var document = new ITextDocument(pdf);

                    var title = new ITextParagraph($"Payroll Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(16)
                        .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD));
                    document.Add(title);
                    document.Add(new ITextParagraph("\n"));

                    var table = new ITextTable(new float[] { 3, 2, 2, 2, 2, 2, 2, 2 });
                    table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                    string[] headers = { "Employee", "Period Start", "Period End", "Basic", "Allowances", "Deductions", "Tax", "Net" };
                    foreach (var header in headers)
                    {
                        table.AddHeaderCell(new ITextCell().Add(new ITextParagraph(header).SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))));
                    }

                    foreach (var item in payrolls)
                    {
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.Employee.FullName)));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.PeriodStart.ToString("yyyy-MM-dd"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.PeriodEnd.ToString("yyyy-MM-dd"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.BasicSalary.ToString("N2"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.Allowances.ToString("N2"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.Deductions.ToString("N2"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.Tax.ToString("N2"))));
                        table.AddCell(new ITextCell().Add(new ITextParagraph(item.NetSalary.ToString("N2"))));
                    }

                    document.Add(table);
                    document.Close();
                }

                string fileName = $"PayrollReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
                return File(stream.ToArray(), "application/pdf", fileName);
            }
        }

        //public IActionResult SalaryExportToPdf(DateTime startDate, DateTime endDate)
        //{
        //    var payrolls = _context.Payrolls
        //        .Where(p => p.PeriodStart >= startDate && p.PeriodEnd <= endDate)
        //        .Include(p => p.Employee)
        //        .ToList();

        //    using (var stream = new MemoryStream())
        //    {
        //        var doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10);
        //        PdfWriter.GetInstance(doc, stream);
        //        doc.Open();

        //        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        //        var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        //        doc.Add(new iTextSharp.text.Paragraph($"Payroll Report ({startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd})", titleFont));
        //        doc.Add(new iTextSharp.text.Paragraph("\n"));

        //        var table = new PdfPTable(8) { WidthPercentage = 100 };
        //        table.AddCell("Employee");
        //        table.AddCell("Period Start");
        //        table.AddCell("Period End");
        //        table.AddCell("Basic");
        //        table.AddCell("Allowances");
        //        table.AddCell("Deductions");
        //        table.AddCell("Tax");
        //        table.AddCell("Net");

        //        foreach (var item in payrolls)
        //        {
        //            table.AddCell(item.Employee.FullName);
        //            table.AddCell(item.PeriodStart.ToString("yyyy-MM-dd"));
        //            table.AddCell(item.PeriodEnd.ToString("yyyy-MM-dd"));
        //            table.AddCell(item.BasicSalary.ToString("N2"));
        //            table.AddCell(item.Allowances.ToString("N2"));
        //            table.AddCell(item.Deductions.ToString("N2"));
        //            table.AddCell(item.Tax.ToString("N2"));
        //            table.AddCell(item.NetSalary.ToString("N2"));
        //        }

        //        doc.Add(table);
        //        doc.Close();

        //        string fileName = $"PayrollReport_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
        //        return File(stream.ToArray(), "application/pdf", fileName);
        //    }
        //}

    }
}
