using DocumentFormat.OpenXml.Spreadsheet;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.Http.Headers;
using System.Text.Json;
[Authorize]
[ApiController]
[Produces("application/json")]
[Route("paypal")]
public class PayPalController : ControllerBase
{
    private readonly PayPalService _pp;
    private readonly ApplicationDbContext _context;
    private readonly LicenseService _licenseService;
    private readonly EmailService _emailService;
    public PayPalController(PayPalService pp, ApplicationDbContext context, LicenseService licenseService,EmailService emailService)
    {
        _pp = pp;
        _context = context;
        _licenseService = licenseService;
        _emailService = emailService;
    }

    // ===============================
    // 1) Create PayPal Order
    // ===============================
    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req)
    {
        try
        {
            string orderId = await _pp.CreateOrder(req.Amount, req.Currency ?? "USD");
            return Ok(new { orderID = orderId });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                status = "error",
                message = ex.Message
            });
        }
    }

    // ===============================
    // 2) Capture PayPal Order
    // ===============================
    [HttpPost("capture-order")]
    public async Task<IActionResult> CaptureOrder([FromBody] CaptureOrderRequest req)
    {
        try
        {
            // محاولة عمل Capture
            JsonElement response = await _pp.CaptureOrder(req.OrderID);
            var email = User.GetUserEmail();
            var userId=User.GetUserId();
            string status = response.GetProperty("status").GetString();

            if (status == "COMPLETED")
            {
                await SavePaymentIfNotExists(response, req.OrderID);
              var license=  await _licenseService.AddLicense(req.OrderID,email , userId);
                await _emailService.SendEmailAsync(
        email,
        "Your License Key",$"Thank You for your purchase, Your license key is: {req.OrderID}"
    );
                return Ok(new { status = "success" });
            }

            return BadRequest(new
            {
                status = "failed",
                message = "Payment not completed"
            });
        }
        catch (Exception ex)
        {
            // PayPal يعيد ORDER_ALREADY_CAPTURED داخل نص الرسالة JSON
            if (ex.Message.Contains("ORDER_ALREADY_CAPTURED"))
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(x => x.OrderId == req.OrderID);

                if (payment != null)
                {
                    return Ok(new
                    {
                        status = "already_captured",
                        message = "Order was already captured earlier.",
                        paymentId = payment.Id
                    });
                }

                // إذا PayPal عمل capture لكن DB فاضية → نعمل استعادة
                var details = await GetOrderDetails(req.OrderID);
                await SavePaymentIfNotExists(details, req.OrderID);

                return Ok(new
                {
                    status = "restored_from_paypal",
                    message = "Payment restored and saved."
                });
            }

            return BadRequest(new { status = "error", message = ex.Message });
        }
    }

    // ===============================
    // Helper: Get Order Details
    // ===============================
    private async Task<JsonElement> GetOrderDetails(string orderId)
    {
        var token = await _pp.GetAccessTokenForController();
        var client = _pp.GetHttpClient();
        var baseUrl = _pp.GetBaseUrl();

        var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/v2/checkout/orders/{orderId}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await client.SendAsync(req);

        var json = await res.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    // ===============================
    // Helper: Save Payment
    // ===============================
    private async Task SavePaymentIfNotExists(JsonElement data, string orderId)
    {
        var existing = await _context.Payments.FirstOrDefaultAsync(x => x.OrderId == orderId);
        var userId = User.GetUserId(); ;
        if (existing != null)
            return;

        var capture = data
            .GetProperty("purchase_units")[0]
            .GetProperty("payments")
            .GetProperty("captures")[0];

        var payment = new Payment
        {
            OrderId = orderId,
            CaptureId = capture.GetProperty("id").GetString(),
            Amount = capture.GetProperty("amount").GetProperty("value").GetString() is string val ? decimal.Parse(val) : 0,
            Currency = capture.GetProperty("amount").GetProperty("currency_code").GetString(),
            Status = capture.GetProperty("status").GetString(),
            UserId = userId,
            CreatedById=userId,
            CreatedOn=DateTime.Now,
            ModifiedById=userId,
            ModifiedOn=DateTime.Now
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(userId);
    }
}

public class CreateOrderRequest { public decimal Amount { get; set; } public string? Currency { get; set; } }
public class CaptureOrderRequest { public string OrderID { get; set; } = default!; }
