
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

