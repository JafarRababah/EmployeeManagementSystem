using EmployeesManagment.Data;
using EmployeesManagment.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EmployeesManagment.Services
{
    public class LicenseService
    {
        private readonly ApplicationDbContext _context;
        private const string secretKey = "JafarHashKey";
        private readonly UserManager<ApplicationUser> _userManager;
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
         .FirstOrDefault(l => l.LicenseKey == computedHash && l.IsActive);
            if (license == null)
                return false;
            return license.ExpiryDate >= DateTime.UtcNow;
        }

        public async Task<License> AddLicense(string licenseKey, string email, string userId=null)
        {
            // جلب المستخدم
            
            if (userId == null)
            {
                throw new Exception($"User '{userId}' not found.");
            }
            
            // إنشاء الترخيص
            var license = new License
            {
                LicenseKey = GenerateLicenseHash(licenseKey),
                ClientEmail = email,    // لاحظ U كابيتال
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true,          // أو IsActive = true إذا عندك عمود Boolean
                CreatedAt = DateTime.UtcNow
            };

             _context.Licenses.Add(license);
            await _context.SaveChangesAsync(userId);

            Console.WriteLine("License saved in DB: " + license.Id);
            return license;
        }
        public async Task<License> UpdateLicense(string licenseKey, string userId = null)
        {
            // جلب المستخدم

            if (userId == null)
            {
                throw new Exception($"User '{userId}' not found.");
            }
            if (licenseKey == null)
            {
                throw new Exception($"licenseKey '{userId}' not found.");
            }
            // إنشاء الترخيص
            var license = new License
            {
                LicenseKey = GenerateLicenseHash(licenseKey),
                ClientEmail = licenseKey,    // لاحظ U كابيتال
                IsActive = true,          // أو IsActive = true إذا عندك عمود Boolean
                CreatedAt = DateTime.UtcNow
            };

            _context.Licenses.Update(license);
            await _context.SaveChangesAsync(userId);

            Console.WriteLine("License saved in DB: " + license.Id);
            return license;
        }
        public License GetLicense(string licenseKey)
        {
            return _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);
        }
    }
}
