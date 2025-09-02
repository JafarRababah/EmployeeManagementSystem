using EmployeesManagment.Data;
using EmployeesManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Data.Seed
{
    public static class FakeDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            var context = sp.GetRequiredService<ApplicationDbContext>();

            if (!await context.Countries.AnyAsync())
            {
                context.Countries.AddRange(
                    new Country { Name = "Jordan" },
                    new Country { Name = "Qatar" },
                    new Country { Name = "UAE" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department { Name = "IT" },
                    new Department { Name = "HR" },
                    new Department { Name = "Finance" }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.Designations.AnyAsync())
            {
                context.Designations.AddRange(
                    new Designation { Name = "Software Engineer" },
                    new Designation { Name = "HR Manager" },
                    new Designation { Name = "Accountant" }
                );
                await context.SaveChangesAsync();
            }

            // „À«· ⁄·Ï ≈÷«›… „ÊŸ›Ì‰  Ã—Ì»ÌÌ‰
            if (!await context.Employees.AnyAsync())
            {
                context.Employees.AddRange(
                    new Employee
                    {
                        FirstName = "Ali",
                        LastName = "Omar",
                        EmpNo = "EMP001",
                        DepartmentId = 1,
                        DesignationId = 1,
                        StatusId = 1,
                        DateOfBirth = new DateTime(1990, 5, 20),
                        EmploymentDate = DateTime.Now.AddYears(-2)
                    },
                    new Employee
                    {
                        FirstName = "Sara",
                        LastName = "Ahmad",
                        EmpNo = "EMP002",
                        DepartmentId = 2,
                        DesignationId = 2,
                        StatusId = 1,
                        DateOfBirth = new DateTime(1992, 8, 15),
                        EmploymentDate = DateTime.Now.AddYears(-1)
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
