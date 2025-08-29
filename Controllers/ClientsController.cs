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
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Clients.Include(c => c.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            var statusId = await _context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "ClientStatus" && x.Code == "Active").FirstOrDefaultAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                
                    client.CreatedOn = DateTime.Now;
                    client.CreatedById = User.Identity.Name;
                    client.StatusId = statusId.Id;
                    client.ModifiedById= User.Identity.Name;
                    client.ModifiedOn = DateTime.Now;
                    _context.Add(client);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Client created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex) {
                TempData["Error"] = "Error creating Client " + ex.Message;
                ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", client.Status);
                return View(client);
            }
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", client.StatusId);
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Client client)
        {
            var userId = User.GetUserId();
            if (id != client.Id)
            {
                return NotFound();
            }

            
                try
                {
                client.ModifiedById = User.GetUserName();
                    client.ModifiedOn = DateTime.Now;
                    _context.Update(client);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Client upddated successfully ";

                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            
            TempData["Error"] = "Client not updated";
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", client.StatusId);
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
