using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace EmployeesManagment.Services
{
    public static class ProfileUserService
    {
        public static string GetUserId(this ClaimsPrincipal user)

        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentLoggedInUser = user;
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetUserName(this ClaimsPrincipal user)

        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentLoggedInUser = user;
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.FindFirst(ClaimTypes.Name).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetUserEmail(this ClaimsPrincipal user)

        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentLoggedInUser = user;
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.FindFirst(ClaimTypes.Email).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetRoleId(this ClaimsPrincipal user)

        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentLoggedInUser = user;
                if (currentLoggedInUser != null)
                {
                    return currentLoggedInUser.FindFirst("RoleId").Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
