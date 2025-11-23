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
        var path = (context.HttpContext.Request.Path.Value ?? "").ToLower();

        // السماح لمسار الصفحة الرئيسية فقط إذا كان يعيد توجيه المستخدم إلى Landing
        if (path == "/")
            return;

        // السماح لصفحة Landing
        if (path == "/home/landing")
            return;
       
        // السماح لصفحة الترخيص
        if (path == "/licenses/enterlicense")
            return;
        // السماح لصفحة login
        if (path.StartsWith("/account/login"))
            return;
        // السماح لصفحة register
        if (path.StartsWith("/account/register"))
            return;
        // السماح لصفحة paypal
        if (path.StartsWith("/paypal"))
            return;

        // السماح لجميع صفحات الدفع
        if (path.StartsWith("/payment"))
            return;

        // السماح لأي API
        if (path.StartsWith("/api/"))
            return;

        // السماح للملفات الثابتة (CSS, JS, IMAGES)
        if (path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") || path.StartsWith("/images"))
            return;

        // السماح لأي API
        if (path.StartsWith("/api/"))
            return;

        // فحص الترخيص للصفحات الداخلية
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
