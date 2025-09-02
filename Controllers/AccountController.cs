using Microsoft.AspNetCore.Mvc;

namespace EmployeesManagment.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
