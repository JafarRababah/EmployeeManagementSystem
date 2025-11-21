using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EmployeesManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly LicenseService _licenseService;

        public HomeController(ApplicationDbContext context,ILogger<HomeController> logger, LicenseService licenseService)
        {
            _context = context;
            _logger = logger;
            _licenseService = licenseService;
        }

        public IActionResult Index()
        {
            //if (TempData["LicenseKey"] == null)
            //{
            //    return RedirectToAction("EnterLicense", "Licenses");
            //}

            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Redirect("~/identity/account/login");
            //}

            //// 👇 بدل View() إلى:
            //return RedirectToAction("Landing");
            return RedirectToAction("Landing");
        }

        public IActionResult Landing()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            // Pending Leaves
            var pendingLeavesCount = await _context.LeaveApplications
                .Where(x => x.Status != null && x.Status.Code == "Pending")
                .CountAsync();
            int pendingLeaves = pendingLeavesCount; // CountAsync آمن ولن يرجع null

            // New Employees هذا الشهر
            var newEmployeesCount = await _context.Employees
                .Where(x => x.CreatedOn.Month == DateTime.Now.Month && x.CreatedOn.Year == DateTime.Now.Year)
                .CountAsync();
            int newEmployees = newEmployeesCount;

            // Late Employees Rate (قد يكون حسابه null من المصدر، هنا مثال مؤقت)
            int? lateEmployeesRate = 25; // افتراضي، يمكن تغييره ليحسب فعليًا

            // Total Salary للشهر الحالي
            var totalSalarySum = await _context.Payrolls
                .Where(p => p.PeriodStart.Month == DateTime.Now.Month && p.PeriodStart.Year == DateTime.Now.Year)
                .SumAsync(p => (decimal?)p.NetSalary); // قد يكون null إذا لا توجد بيانات
            int totalSalary = (int)(totalSalarySum ?? 0); // null تتحول إلى صفر
                                                          // بيانات الرسوم البيانية (آخر 6 شهور)
            var months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .Select(d => d.ToString("MMM"))
                .ToList();
            var attendanceData = new List<int>();
            var leaveData = new List<int>();
            foreach (var month in months)
            {
                var monthDate = DateTime.ParseExact(month, "MMM", System.Globalization.CultureInfo.InvariantCulture);
                int m = monthDate.Month;
                int y = DateTime.Now.Year;

                int attendanceCount = await _context.Attendances
                    .CountAsync(a => a.Date.Month == m && a.Date.Year == y);
                int leaveCount = await _context.LeaveApplications
                    .CountAsync(l => l.StartDate.Month == m && l.StartDate.Year == y);

                attendanceData.Add(attendanceCount);
                leaveData.Add(leaveCount);
            }
            // إنشاء الـ ViewModel
            var model = new DashboardViewModel
            {
                PendingLeaves = pendingLeaves,
                NewEmployees = newEmployees,
                LateRate = lateEmployeesRate ?? 0, // null تتحول إلى صفر
                TotalSalary = totalSalary,
                Months = months,
                AttendanceData = attendanceData,
                LeaveData = leaveData
            };

            return View("Index", model);
        }


        public IActionResult Demo()
        {
            // ✅ مثال: يمكنك هنا إظهار صفحة Dashboard تجريبية
            // أو صفحة تحتوي على بيانات Mock لتجربة AttendPro
            var demoModel = new DashboardViewModel
            {
                PendingLeaves = 5,
                NewEmployees = 3,
                LateRate = 12,
                TotalSalary = 15000,
                Months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                AttendanceData = new List<int> { 90, 85, 88, 92, 94, 95 },
                LeaveData = new List<int> { 10, 15, 12, 8, 6, 5 }
            };

            return View("Index", demoModel); // إعادة استخدام الـ Dashboard View
        }
        // 🔹 API لتزويد البيانات الخاصة بالرسوم البيانية (تُستدعى بـ AJAX)
        [HttpGet]
        public async Task<IActionResult> GetChartsData()
        {
            var months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Now.AddMonths(-i))
                .OrderBy(d => d)
                .Select(d => d.ToString("MMM"))
                .ToList();

            var attendanceData = new List<int>();
            var leaveData = new List<int>();

            foreach (var month in months)
            {
                var monthDate = DateTime.ParseExact(month, "MMM", System.Globalization.CultureInfo.InvariantCulture);
                int m = monthDate.Month;
                int y = DateTime.Now.Year;

                int attendanceCount = await _context.Attendances
                    .CountAsync(a => a.Date.Month == m && a.Date.Year == y);
                int leaveCount = await _context.LeaveApplications
                    .CountAsync(l => l.StartDate.Month == m && l.StartDate.Year == y);

                attendanceData.Add(attendanceCount);
                leaveData.Add(leaveCount);
            }

            return Json(new
            {
                months,
                attendanceData,
                leaveData,
                leaveDistribution = new { Annual = 35, Sick = 25, Unpaid = 20, Other = 20 } // مثال
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
