using AutoMapper;
using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public CountriesController(ApplicationDbContext context,IMapper mapper,IExtensionService extensionService)
        {
            _context = context;
            _mapper = mapper;
            _extesionService = extensionService;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            var country = await _context.Countries.ToListAsync();
            return View(country);
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                    country.Code = await _extesionService.GenerateCountryNumber();
                    country.CreatedById = User.Identity.Name;
                    country.CreatedOn = DateTime.Now;
                    _context.Add(country);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Country created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating Country " + ex.Message;
                return View(country);
            }
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Country country)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != country.Id)
            {
                return NotFound();
            }
            if (!CountryExists(country.Id))
            {
                return NotFound();
            }
            ModelState.Remove("CreatedById");
            ModelState.Remove("ModifiedOn");
            
                try
                {
                    country.ModifiedOn = DateTime.Now;
                    country.ModifiedById = User.Identity.Name;
                    _context.Update(country);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Country updated successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
                {
                    
                TempData["Error"] = "Error updated Country " + ex.Message;
                return View(country);
            }

        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }
            try
            {
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Country deleted successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error delete country " + ex.Message;
                return View(country);
            }
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
