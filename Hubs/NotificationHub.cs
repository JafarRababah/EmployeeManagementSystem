using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Hubs
{
    // اختياري: لو عندك Authentication
    [Authorize]
    public class NotificationHub : Hub
    {
        // يُستدعى عند اتصال عميل جديد
        public override async Task OnConnectedAsync()
        {
            // مثال اختياري: ضمّ المستخدم لمجموعة حسب دوره
            if (Context.User?.IsInRole("Admin") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }

        // يُستدعى عند انقطاع الاتصال
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // مثال اختياري: إزالة من المجموعة
            if (Context.User?.IsInRole("Admin") == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // إرسال إشعار لمستخدم معيّن (سيتم استقبال الحدث "ReceiveNotification" على الواجهة)
        public async Task SendToUser(string userId, string message, string? url = null)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message, url);
        }

        // إرسال إشعار لكل المدراء (المضافين لمجموعة "Admins")
        public async Task SendToAdmins(string message, string? url = null)
        {
            await Clients.Group("Admins").SendAsync("ReceiveNotification", message, url);
        }

        // إرسال إشعار للجميع
        public async Task SendToAll(string message, string? url = null)
        {
            await Clients.All.SendAsync("ReceiveNotification", message, url);
        }
    }
}
