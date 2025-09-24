using EmployeesManagment.Models;
using EmployeesManagment.Data;
using System.Linq;

namespace EmployeesManagment.Services
{
    public class LicenseService
    {
        private readonly ApplicationDbContext _context;

        public LicenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsLicenseValid(string licenseKey)
        {
            var license = _context.Licenses
                .FirstOrDefault(l => l.LicenseKey == licenseKey && l.IsActive);

            if (license == null) return false;

            return license.ExpiryDate >= DateTime.UtcNow;
        }
        public License GetLicense(string licenseKey)
        {
            return _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);
        }
    }
}
