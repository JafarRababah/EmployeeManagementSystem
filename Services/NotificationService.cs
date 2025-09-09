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
    }
}
