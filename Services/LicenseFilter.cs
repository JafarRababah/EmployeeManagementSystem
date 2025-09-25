using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class LicenseFilter : IActionFilter
{
    private readonly LicenseService _licenseService;

    public LicenseFilter(LicenseService licenseService)
    {
        _licenseService = licenseService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var path = context.HttpContext.Request.Path.Value.ToLower();

        // السماح بالوصول إلى صفحة EnterLicense
        if (path.Contains("/licenses/enterlicense"))
            return;

        var hasLicense = context.HttpContext.Session.GetString("LicenseKey");
        if (string.IsNullOrEmpty(hasLicense) || !_licenseService.IsLicenseValid(hasLicense))
        {
            context.Result = new RedirectToActionResult("EnterLicense", "Licenses", null);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // لا شيء بعد التنفيذ
    }
}
