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
            if (TempData["LicenseKey"] == null)
            {
                return RedirectToAction("EnterLicense", "Licenses");
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("~/identity/account/login");
            }

            // 👇 بدل View() إلى:
            return RedirectToAction("Dashboard");

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

            // إنشاء الـ ViewModel
            var model = new DashboardViewModel
            {
                PendingLeaves = pendingLeaves,
                NewEmployees = newEmployees,
                LateRate = lateEmployeesRate ?? 0, // null تتحول إلى صفر
                TotalSalary = totalSalary
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
                TotalSalary = 15000
            };

            return View("Dashboard", demoModel); // إعادة استخدام الـ Dashboard View
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
