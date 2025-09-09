using EmployeesManagment.Data;
using EmployeesManagment.Hubs;
using EmployeesManagment.Models;
using Microsoft.AspNetCore.SignalR;

public class NotificationService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task AddNotificationAsync(string userId, string message, string url = null)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Url = url,
            IsRead = false,
            CreatedOn = DateTime.Now
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // إرسال الإشعار مباشرة عبر SignalR
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            message = message,
            url = url,
            createdOn = notification.CreatedOn.ToString("g")
        });
    }

}
