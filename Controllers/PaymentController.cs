using EmployeesManagment.Data;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeesManagment.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayPalService _paypalService;
        private readonly ApplicationDbContext _context;

        public PaymentController(PayPalService paypalService, ApplicationDbContext context)
        {
            _paypalService = paypalService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ================================


        // ================================
        // 2) Success Redirect
        // ================================
        [HttpGet("payment/success")]
        public async Task<IActionResult> Success(string orderId)
        {
            try
            {
                var userId = "0996e9a3-c4a0-476c-a945-d2e68e8edc97";
                var existingPayment = _context.Payments.FirstOrDefault(x => x.OrderId == orderId);
                if (existingPayment != null)
                {
                    ViewBag.OrderId = existingPayment.OrderId;
                    ViewBag.CaptureId = existingPayment.CaptureId;
                    return View();
                }
                JsonElement capture = await _paypalService.CaptureOrder(orderId);

                var captureId = capture
                    .GetProperty("purchase_units")[0]
                    .GetProperty("payments")
                    .GetProperty("captures")[0]
                    .GetProperty("id")
                    .GetString();

                var amount = capture
                    .GetProperty("purchase_units")[0]
                    .GetProperty("payments")
                    .GetProperty("captures")[0]
                    .GetProperty("amount")
                    .GetProperty("value")
                    .GetString();

                var payment = new Payment
                {
                    OrderId = orderId,
                    CaptureId = captureId,
                    Amount = decimal.Parse(amount),
                    Currency = "USD",
                    Status = "Completed"
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(userId);

                ViewBag.OrderId = orderId;
                ViewBag.CaptureId = captureId;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Failed", new { message = ex.Message });
            }
        }


        // ================================
        // 3) Cancel Redirect
        // ================================
        [HttpGet]
        public async Task<IActionResult> Cancel(string token)
        {
            var userId = User.GetUserId();
            var payment = new Payment
            {
                OrderId = token,
                Status = "Canceled"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(userId);

            return View();
        }

        // ================================
        // 4) Failed Page
        // ================================
        public IActionResult Failed(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}
