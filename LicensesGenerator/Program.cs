using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace LicenseGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== License Generator Tool ===");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=.;Database=EmployeeManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var licenseService = new LicenseService(context);

            Console.Write("Enter License Key (leave blank to auto-generate): ");
            var licenseKey = Console.ReadLine();
            Console.Write("Enter username: ");
            var userName = Console.ReadLine();

            var user = context.Users.FirstOrDefault(u => u.UserName == userName);
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
                Console.WriteLine($"Generated Key: {licenseKey}");
            }

            await licenseService.AddLicense(licenseKey, userName, user.Id);
            Console.WriteLine("License added successfully!");
        }
    }
}
