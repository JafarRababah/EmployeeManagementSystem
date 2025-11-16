using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("paypal")]
public class PayPalController : ControllerBase
{
    private readonly PayPalService _pp;
    public PayPalController(PayPalService pp) => _pp = pp;

    [HttpPost("create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest req)
    {
        // req.amount مثلاً 29.00
        var orderId = await _pp.CreateOrder(req.Amount, req.Currency ?? "USD");
        return Ok(new { orderID = orderId });
    }

    [HttpPost("capture-order")]
    public async Task<IActionResult> CaptureOrder([FromBody] CaptureOrderRequest req)
    {
        var result = await _pp.CaptureOrder(req.OrderID);
        return Ok(result); // يرجع JSON النتيجة من PayPal
    }
}

public class CreateOrderRequest { public decimal Amount { get; set; } public string? Currency { get; set; } }
public class CaptureOrderRequest { public string OrderID { get; set; } = default!; }
