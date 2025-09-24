using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeesManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LicenseService _licenseService;

        public HomeController(ILogger<HomeController> logger, LicenseService licenseService)
        {
            _logger = logger;
            _licenseService = licenseService;
        }

        public IActionResult Index()
        {
            string licenseKey = "ABC123-XYZ789"; // Ì„ﬂ‰  Œ“Ì‰Â ›Ì appsettings √Ê ≈œŒ«·Â „‰ «·„” Œœ„

            //if (!_licenseService.IsLicenseValid(licenseKey))
            //{
            //    Console.WriteLine("License is invalid or expired!");
            //    // ≈Ìﬁ«› «·‰Ÿ«„ √Ê ≈⁄«œ…  ÊÃÌÂ «·„” Œœ„
            //}
            if (TempData["LicenseKey"] == null)
            {
                return RedirectToAction("EnterLicense", "Licenses");
            }

            return !User.Identity.IsAuthenticated
                ? this.Redirect("~/identity/account/login")
                : View();


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
