using System;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Services;

namespace LicenseGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== License Generator Tool ===");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=.;Database=EmployeeManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;")
                .Options;

            using var context = new ApplicationDbContext(options);
            var licenseService = new LicenseService(context);

            Console.Write("Enter License Key (leave blank to auto-generate): ");
            var licenseKey = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                licenseKey = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();
                Console.WriteLine($"Generated Key: {licenseKey}");
            }

            Console.Write("Enter expiry date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime expiryDate))
            {
                expiryDate = DateTime.UtcNow.AddYears(1);
                Console.WriteLine($"Default expiry used: {expiryDate}");
            }

            licenseService.AddLicense(licenseKey, expiryDate);
            Console.WriteLine("License added successfully!");
        }
    }
}
