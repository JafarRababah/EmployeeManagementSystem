//using EmployeesManagment.Infrastructure;
//using EmployeesManagment.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Threading.Tasks;

//namespace EmployeesManagment.Infrastructure
//{
//    public static class DbSeeder
//    {
//        public static async Task SeedRolesAndUsersAsync(IServiceProvider services)
//        {
//            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();
//            var userMgr = services.GetRequiredService<UserManager<IdentityUser>>(); // Change to ApplicationUser if you have a custom class

//            string[] roles = { Roles.Admin, Roles.HR, Roles.Employee };
//            foreach (var role in roles)
//                if (!await roleMgr.RoleExistsAsync(role))
//                    await roleMgr.CreateAsync(new IdentityRole(role));

//            async Task EnsureUser(string email, string pass, string role)
//            {
//                var user = await userMgr.FindByEmailAsync(email);
//                if (user == null)
//                {
//                    user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
//                    await userMgr.CreateAsync(user, pass);
//                }
//                if (!await userMgr.IsInRoleAsync(user, role))
//                    await userMgr.AddToRoleAsync(user, role);
//            }

//            await EnsureUser("admin@demo.local", "Admin#12345", Roles.Admin);
//            await EnsureUser("hr@demo.local", "Hr#12345", Roles.HR);
//            await EnsureUser("employee@demo.local", "Emp#12345", Roles.Employee);
//        }
//    }
//}
using EmployeesManagment.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeesManagment.Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider sp)
        {
            var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

            // ≈‰‘«¡ «·√œÊ«— ≈–« ·„  ﬂ‰ „ÊÃÊœ…
            var roles = new[] { "Admin", "HR", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ≈‰‘«¡ „” Œœ„ „”ƒÊ· ≈–« ·„ Ìﬂ‰ „ÊÃÊœ
            var adminEmail = "admin@hr.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };
                await userManager.CreateAsync(admin, "Admin@123"); // »«”Ê—œ „»œ∆Ì
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }


        
          
       
    }
}

