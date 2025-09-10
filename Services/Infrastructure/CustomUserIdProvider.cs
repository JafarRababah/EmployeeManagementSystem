using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EmployeesManagment.Infrastructure
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // الأفضل استخدام ClaimTypes.NameIdentifier حتى يطابق AspNetUsers.Id
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
