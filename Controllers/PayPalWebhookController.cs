using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly LicenseService _licenseService;
        private readonly EmailService _emailService;

        public PayPalWebhookController(ApplicationDbContext context, LicenseService licenseService, EmailService emailService)
        {
            _context = context;
            _licenseService = licenseService;
            _emailService = emailService;
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            string body;
            using (var reader = new StreamReader(Request.Body))
                body = await reader.ReadToEndAsync();

            try
            {
                var json = JObject.Parse(body);
                var eventType = json["event_type"]?.ToString();
                var payerEmail = json["resource"]?["payer"]?["email_address"]?.ToString();
                var amount = json["resource"]?["amount"]?["value"]?.ToString();
                var status = json["resource"]?["status"]?.ToString();

                // ✅ تحقق من الدفع الناجح
                if (eventType == "PAYMENT.CAPTURE.COMPLETED" || status == "COMPLETED")
                {
                    // 🔑 توليد LicenseKey فريد
                    var licenseKey = GenerateLicenseKey(payerEmail);
                    var userId = "ca341927-ae4c-4279-9a94-eba913954ad1";
                    // 🧾 حفظ الترخيص في قاعدة البيانات
                    var license = new License
                    {
                        LicenseKey = licenseKey,
                        ClientEmail = payerEmail ?? "j3fr.rababah@gmail.com",
                        ExpiryDate = DateTime.UtcNow.AddYears(1),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Licenses.Add(license);
                    await _context.SaveChangesAsync(userId);
                    //                    // ✉️ إرسال البريد إلى المشتري
                    string subject = "Your AttendPro License Key";
                    string downloadUrl = "https://a1b2c3d4.ngrok.io/api/paypalwebhook/webhook"; // استبدلها بالرابط الفعلي
                    string emailBody = $@"
    <h2>🎉 شكراً لشرائك AttendPro!</h2>
    <p>مرحباً {payerEmail},</p>
    <p>إليك تفاصيل طلبك:</p>
    <ul>
        <li><strong>المبلغ:</strong> {amount}$</li>
        <li><strong>License Key:</strong> <code>{licenseKey}</code></li>
        <li><strong>رابط التحميل:</strong> <a href='{downloadUrl}'>Download AttendPro</a></li>
    </ul>
    <p>نشكرك على دعمك 🙏<br>فريق AttendPro</p>
";

                    await _emailService.SendEmailAsync(payerEmail, subject, emailBody);

                    // ✉️ إرسال البريد للمشتري (سيضاف لاحقاً)
                    System.IO.File.AppendAllText("paypal_log.txt",
                        $"✅ License generated for {payerEmail} → {licenseKey}{Environment.NewLine}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("paypal_log.txt", $"❌ Error: {ex.Message}\n");
                return BadRequest(ex.Message);
            }
        }

        // 🔐 توليد LicenseKey بسيط من البريد والتاريخ
        private string GenerateLicenseKey(string email)
        {
            var baseStr = $"{email}-{Guid.NewGuid()}-{DateTime.UtcNow}";
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(baseStr));
                return BitConverter.ToString(hash).Replace("-", "").Substring(0, 20);
            }
        }

        [HttpGet("webhook")]
        public IActionResult Test()
        {
            return Ok("PayPal Webhook is active and waiting for POST requests.");
        }
    }
}
