using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class LicensesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly LicenseService _licenseService;
        private const string secretKey = "JafarHashKey";
        private readonly EmailService _emailService;

        public LicensesController(ApplicationDbContext context, LicenseService licenseService, EmailService emailService)
        {
            _context = context;
            _licenseService = licenseService;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult EnterLicense()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnterLicense(string licenseKey)
        {
            if (_licenseService.IsLicenseValid(licenseKey))
            {
                TempData["LicenseKey"] = licenseKey;
                HttpContext.Session.SetString("LicenseKey", licenseKey);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid or expired license key!");
            return View();
        }

        // GET: Licenses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Licenses.ToListAsync());
        }

        // GET: Licenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // GET: Licenses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licenses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(License license)
        {
            try
            {
                _context.Add(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception ex)
            {
                return View(license);
            }
        }

        // GET: Licenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses.FindAsync(id);
            if (license == null)
            {
                return NotFound();
            }
            return View(license);
        }
        private string GenerateLicenseHash(string licenseKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(licenseKey));
                return Convert.ToBase64String(hash);
            }
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
        // POST: Licenses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  License license)
        {
            if (id != license.Id)
            {
                return NotFound();
            }

            
                try
                {
                license.LicenseKey = GenerateLicenseHash(license.LicenseKey);
                _context.Update(license);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseExists(license.Id))
                    {
                        return NotFound();
                    }
                   
                return View(license);
            }
            
           
        }

        // GET: Licenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // POST: Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var license = await _context.Licenses.FindAsync(id);
            if (license != null)
            {
                _context.Licenses.Remove(license);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseExists(int id)
        {
            return _context.Licenses.Any(e => e.Id == id);
        }
        [HttpGet]
        public async Task<IActionResult> MyLicense()
        {
            var userEmail = User.GetUserEmail();

            var license = await _context.Licenses
                .Where(l => l.ClientEmail == userEmail)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            return View(license);
        }

        [HttpPost]
        public async Task<IActionResult> ResendLicense()
        {
            var userEmail = User.GetUserEmail();
            var license = await _context.Licenses
                .Where(l => l.ClientEmail == userEmail)
                .OrderByDescending(l => l.CreatedAt)
                .FirstOrDefaultAsync();

            if (license != null)
            {
                await _emailService.SendEmailAsync(
                    userEmail,
                    "Your License Key (Resent)",
                    $"Your License Key: {license.LicenseKey}"
                );
            }

            TempData["Message"] = "License key resent to your email.";
            return RedirectToAction("MyLicense");
        }
    }
}
