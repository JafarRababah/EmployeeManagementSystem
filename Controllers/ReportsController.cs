using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.ViewModels;
using EmployeesManagment.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace EmployeesManagment.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ÚÑÖ ÕÝÍÉ Reports
        public IActionResult Index()
        {
            var vm = new EmployeeReportFilterVM
            {
                Departments = _context.Departments
                                      .Select(d => new SelectListItem
                                      {
                                          Value = d.Id.ToString(),
                                          Text = d.Name
                                      }).ToList()
            };

            return View(vm);
        }

        // ÚÑÖ ÇáãæÙÝíä ãÚ ÇáÝáÊÑÉ
        public IActionResult Employees(string? fullName, int? departmentId)
        {
            var query = _context.Employees
                                .Include(e => e.Department)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(fullName) ||
                    e.MiddleName.Contains(fullName) ||
                    e.LastName.Contains(fullName));
            }


            if (departmentId.HasValue)
                query = query.Where(e => e.DepartmentId == departmentId);

            var vm = new EmployeeReportFilterVM
            {
                FullName = fullName,
                DepartmentId = departmentId,
                Employees = query.ToList(),
                Departments = _context.Departments
                                      .Select(d => new SelectListItem
                                      {
                                          Value = d.Id.ToString(),
                                          Text = d.Name
                                      }).ToList()
            };

            return View("Index", vm); // äÑÌÚ áäÝÓ ÇáÕÝÍÉ ãÚ ÇáÈíÇäÇÊ
        }
        public IActionResult Leaves(string? employeeName, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.LeaveApplications
                                .Include(l => l.Employee)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(employeeName))
            {
                query = query.Where(l =>
                    l.Employee.FirstName.Contains(employeeName) ||
                    l.Employee.LastName.Contains(employeeName));
            }

            if (startDate.HasValue)
            {
                query = query.Where(l => l.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(l => l.EndDate <= endDate.Value);
            }

            var vm = new LeaveReportFilterVM
            {
                EmployeeName = employeeName,
                StartDate = startDate,
                EndDate = endDate,
                LeaveApplications = query.ToList()
            };

            return View(vm);
        }


        // ÊäÒíá Excel
        public IActionResult EmployeesExcel(string? fullName, int? departmentId)
        {
            var query = _context.Employees.Include(e => e.Department).AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(fullName) ||
                    e.MiddleName.Contains(fullName) ||
                    e.LastName.Contains(fullName)
                );
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            var employees = query.ToList();


            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Employees");

            var headers = new[] { "Full Name", "Department", "Email", "Phone", "Employment Date" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            int row = 2;
            foreach (var e in employees)
            {
                ws.Cells[row, 1].Value = $"{e.FirstName} {e.MiddleName} {e.LastName}".Trim();
                ws.Cells[row, 2].Value = e.Department?.Name;
                ws.Cells[row, 3].Value = e.EmailAddress;
                ws.Cells[row, 4].Value = e.PhoneNumber.ToString();
                ws.Cells[row, 5].Value = e.EmploymentDate?.ToString("yyyy-MM-dd");
                row++;
            }

            ws.Cells.AutoFitColumns();
            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
        }
    }
}
