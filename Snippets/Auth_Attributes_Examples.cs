using Microsoft.AspNetCore.Authorization;
using EmployeesManagment.Infrastructure;

namespace EmployeesManagment.Snippets
{
    // Example usage on controllers/actions
    [Authorize(Roles = Roles.Admin + "," + Roles.HR)]
    public class OnlyAdminHrController { }

    [Authorize(Roles = Roles.Employee)]
    public class OnlyEmployeeController { }

    // For multiple roles:
    // [Authorize(Roles = Roles.Admin + "," + Roles.HR + "," + Roles.Employee)]
}
