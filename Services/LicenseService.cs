using EmployeesManagment.Data;
using EmployeesManagment.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EmployeesManagment.Services
{
    public class LicenseService
    {
        private readonly ApplicationDbContext _context;
        private const string secretKey = "JafarHashKey";
        public LicenseService(ApplicationDbContext context)
        {
            _context = context;
        }
        // genrate Hash from key 
        private string GenerateLicenseHash(string licenseKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(licenseKey));
                return Convert.ToBase64String(hash);
            }
        }
        public bool IsLicenseValid(string licenseKey)
        {
            var computedHash = GenerateLicenseHash(licenseKey);
            var license = _context.Licenses
         .FirstOrDefault(l => l.LicenseHash == computedHash && l.IsActive);
            if (license == null)
                return false;
            return license.ExpiryDate >= DateTime.UtcNow;
        }

        public void AddLicense(string licenseKey, DateTime expiryDate)
        {
            // في أي مكان (Seeder, Controller, أو Console App)
            //var licenseService = new LicenseService(_context);

            //// مثال: مفتاح ترخيص صالح لمدة سنة
            //string newKey = "ABC123-XYZ789-TEST2025";
            //expiryDate = DateTime.UtcNow.AddYears(1);

            //licenseService.AddLicense(newKey, expiryDate);
            var license = new License
            {
                LicenseKey = GenerateLicenseHash(licenseKey),
                LicenseHash = GenerateLicenseHash(licenseKey),
                ExpiryDate = expiryDate,
                IsActive = true
            };

            _context.Licenses.Add(license);
            _context.SaveChanges();
        }
        public License GetLicense(string licenseKey)
        {
            return _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);
        }
    }
}
