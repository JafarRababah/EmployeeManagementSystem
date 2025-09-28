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
         .FirstOrDefault(l => l.LicenseHash == computedHash && l.IsActive);
            if (license == null)
                return false;
            return license.ExpiryDate >= DateTime.UtcNow;
        }

        //public void AddLicense(string licenseKey, DateTime expiryDate,string userName)
        //{
        //    var user =  _userManager.FindByNameAsync(userName);
            
          
        //    var license = new License
        //    {
        //        LicenseKey = GenerateLicenseHash(licenseKey),
        //        LicenseHash = user.Id,
        //        ExpiryDate = expiryDate,
        //        IsActive = true
        //    };

        //    _context.Licenses.Add(license);
        //    _context.SaveChanges();
        //}
        public async Task AddLicense(string licenseKey, DateTime expiryDate, string userId)
        {
            // جلب المستخدم
            if (userId == null)
            {
                throw new Exception($"User '{userId}' not found.");
            }

            // إنشاء الترخيص
            var license = new License
            {
                LicenseKey = "ABC123",
                LicenseHash = "j3fr.rababah@gmail.com",    // لاحظ U كابيتال
                ExpiryDate = DateTime.UtcNow,
                IsActive = true,          // أو IsActive = true إذا عندك عمود Boolean
                CreatedAt = DateTime.UtcNow
            };

             _context.Licenses.Add(license);
            await _context.SaveChangesAsync("ca341927 - ae4c - 4279 - 9a94 - eba913954ad1");

            Console.WriteLine("License saved in DB: " + license.Id);
        }

        public License GetLicense(string licenseKey)
        {
            return _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);
        }
    }
}
