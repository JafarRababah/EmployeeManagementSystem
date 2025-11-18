using EmployeesManagment.Models;

public class Payment:UserActivity
{
    public int Id { get; set; }

    public string OrderId { get; set; } = default!;
    public string CaptureId { get; set; } = default!; // بعد عملية Capture

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

   

    // لو عندك Users
    public string? UserId { get; set; }
}
