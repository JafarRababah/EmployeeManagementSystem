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
    public class BanksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public BanksController(ApplicationDbContext context,IMapper mapper,IExtensionService extensionService)
        {
            _context = context;
            _mapper=mapper;
            _extesionService=extensionService;
        }

        // GET: Banks
        public async Task<IActionResult> Index()
        {
            var banks = await _context.Banks
                .ToListAsync();
            return View(banks);
        }

        // GET: Banks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        // GET: Banks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Banks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bank bank)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                //_mapper.Map(bank);
                bank.Code = await _extesionService.GenerateBankNumber();
                bank.CreatedOn = DateTime.Now;
                    bank.CreatedById = User.Identity.Name;
                    _context.Add(bank);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "bank created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating bank " + ex.Message;
                return View(bank);
            }
        }

        // GET: Banks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks.FindAsync(id);
            if (bank == null)
            {
                return NotFound();
            }
            return View(bank);
        }

        // POST: Banks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Bank bank)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != bank.Id)
            {
                return NotFound();
            }
            if (!BankExists(bank.Id))
            {
                return NotFound();
            }

            try
                {
                    bank.ModifiedOn = DateTime.Now;
                    bank.ModifiedById = User.Identity.Name;
                    _context.Update(bank);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "bank updated successfully ";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
                {
                TempData["Error"] = "Error updated bank " + ex.Message;
                return View(bank);
            }
            
            
        }

        // GET: Banks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bank = await _context.Banks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bank == null)
            {
                return NotFound();
            }

            return View(bank);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bank = await _context.Banks.FindAsync(id);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
            }

            await _context.SaveChangesAsync(userId);
            TempData["Message"] = "Bank account deleted successfully ";
            return RedirectToAction(nameof(Index));
        }

        private bool BankExists(int id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }
    }
}
