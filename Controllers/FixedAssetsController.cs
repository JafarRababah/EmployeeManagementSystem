using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeesManagment.Data;
using EmployeesManagment.Models;

namespace EmployeesManagment.Controllers
{
    public class FixedAssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FixedAssetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FixedAssets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FixedAssets.Include(f => f.Category).Include(f => f.ResponsibleEmployee).Include(f => f.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FixedAssets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedAsset = await _context.FixedAssets
                .Include(f => f.Category)
                .Include(f => f.ResponsibleEmployee)
                .Include(f => f.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fixedAsset == null)
            {
                return NotFound();
            }

            return View(fixedAsset);
        }

        // GET: FixedAssets/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
           ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description");
            return View();
        }

        // POST: FixedAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FixedAsset fixedAsset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fixedAsset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", fixedAsset.CategoryId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", fixedAsset.ResponsibleEmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", fixedAsset.StatusId);
            return View(fixedAsset);
        }

        // GET: FixedAssets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedAsset = await _context.FixedAssets.FindAsync(id);
            if (fixedAsset == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", fixedAsset.CategoryId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "Id", fixedAsset.ResponsibleEmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", fixedAsset.StatusId);
            return View(fixedAsset);
        }

        // POST: FixedAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssetNo,Description,CategoryId,SerialNo,Model,StatusId,ResponsibleEmployeeId,Photo,Notes,PurchaseDate,CreatedById,CreatedOn,ModifiedById,ModifiedOn")] FixedAsset fixedAsset)
        {
            if (id != fixedAsset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fixedAsset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FixedAssetExists(fixedAsset.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", fixedAsset.CategoryId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "Id", fixedAsset.ResponsibleEmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails, "Id", "Id", fixedAsset.StatusId);
            return View(fixedAsset);
        }

        // GET: FixedAssets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fixedAsset = await _context.FixedAssets
                .Include(f => f.Category)
                .Include(f => f.ResponsibleEmployee)
                .Include(f => f.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fixedAsset == null)
            {
                return NotFound();
            }

            return View(fixedAsset);
        }

        // POST: FixedAssets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fixedAsset = await _context.FixedAssets.FindAsync(id);
            if (fixedAsset != null)
            {
                _context.FixedAssets.Remove(fixedAsset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FixedAssetExists(int id)
        {
            return _context.FixedAssets.Any(e => e.Id == id);
        }
    }
}
