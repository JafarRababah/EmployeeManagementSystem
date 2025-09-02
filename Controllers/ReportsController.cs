using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EmployeesManagment.Infrastructure;
using EmployeesManagment.Services.Reporting;
using EmployeesManagment.ViewModels.Reports;

// NOTE: Replace with your actual DbContext / Entities
using EmployeesManagment.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.HR)]
    public class ReportsController : Controller
    {
        private readonly IReportService _reports;
        private readonly ApplicationDbContext _db; // change if your DbContext has a different name

        public ReportsController(IReportService reports, ApplicationDbContext db)
        {
            _reports = reports;
            _db = db;
        }
        [Authorize(Roles = Roles.Admin + "," + Roles.HR)]
        public IActionResult Index()
        {
            return View(); // íÚÑÖ Reports/Index.cshtml
        }

        [HttpGet("/reports/employees/excel")]
        public async Task<IActionResult> EmployeesExcel()
        {
            var rows = await _db.Employees
    .Include(e => e.Department)
    .Include(e => e.Designation)
    .Select(e => new EmployeeReportRow
    {
        FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
        Department = e.Department.Name,
        JobTitle = e.Designation.Name,
        Email = e.EmailAddress,
        Phone = e.PhoneNumber.ToString(),
        HiredOn = e.EmploymentDate
    })
    .ToListAsync();



            var bytes = await _reports.EmployeesToExcelAsync(rows);
            return File(bytes, _reports.EmployeesExcelContentType, "employees.xlsx");
        }

        [HttpGet("/reports/employees/pdf")]
        public async Task<IActionResult> EmployeesPdf()
        {
            var rows = await _db.Employees
    .Include(e => e.Department)
    .Include(e => e.Designation)
    .Select(e => new EmployeeReportRow
    {
        FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
        Department = e.Department.Name,
        JobTitle = e.Designation.Name,
        Email = e.EmailAddress,
        Phone = e.PhoneNumber.ToString(),
        HiredOn = e.EmploymentDate
    })
    .ToListAsync();


            var bytes = await _reports.EmployeesToPdfAsync(rows);
            return File(bytes, _reports.PdfContentType, "employees.pdf");
        }

        [HttpGet("/reports/leaves/excel")]
        public async Task<IActionResult> LeavesExcel()
        {
            var rows = await _db.LeaveApplications
    .Include(l => l.Employee)
    .Include(l => l.LeaveType)
    .Select(l => new LeaveReportRow
    {
        EmployeeName = l.Employee.FirstName + " " + l.Employee.MiddleName + " " + l.Employee.LastName,
        LeaveType = l.LeaveType.Name,
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        TotalDays = l.NoOfDays,
        Status = l.Status.ToString(),
        //Notes = l.Notes
    })
    .ToListAsync();


            var bytes = await _reports.LeavesToExcelAsync(rows);
            return File(bytes, _reports.EmployeesExcelContentType, "leaves.xlsx");
        }

        [HttpGet("/reports/leaves/pdf")]
        public async Task<IActionResult> LeavesPdf()
        {
            var rows = await _db.LeaveApplications
    .Include(l => l.Employee)
    .Include(l => l.LeaveType)
    .Select(l => new LeaveReportRow
    {
        EmployeeName = l.Employee.FirstName + " " + l.Employee.MiddleName + " " + l.Employee.LastName,
        LeaveType = l.LeaveType.Name,
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        TotalDays = l.NoOfDays,
        Status = l.Status.ToString(),
        //Notes = l.Notes
    })
    .ToListAsync();


            var bytes = await _reports.LeavesToPdfAsync(rows);
            return File(bytes, _reports.PdfContentType, "leaves.pdf");
        }
    }
}
