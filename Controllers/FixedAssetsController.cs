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
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class FixedAssetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public FixedAssetsController(ApplicationDbContext context, IMapper mapper, IExtensionService extensionService)
        {
            _context = context;
            _mapper = mapper;
            _extesionService = extensionService;
        }

        // GET: FixedAssets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FixedAssets.Include(f => f.Category).
                Include(f => f.ResponsibleEmployee).Include(f => f.Status);
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
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCodeValue).Where(x=>x.SystemCodeValue.Code=="AssetCategories"), "Id", "Description");
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
           ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetStatus"), "Id", "Description");
            return View();
        }

        // POST: FixedAssets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FixedAsset fixedAsset)
        {
            var fixedAssetStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(x => x.SystemCodeValue.Code == "AssetStatus" && x.Code == "Active").FirstOrDefaultAsync();
            var userId = User.GetUserId();
            fixedAsset.AssetNo = await _extesionService.GenerateAssetNumber();
            fixedAsset.CreatedById = User.GetUserName();
            fixedAsset.CreatedOn = DateTime.Now;
            fixedAsset.ModifiedById = User.GetUserName();
            fixedAsset.ModifiedOn = DateTime.Now;
            fixedAsset.StatusId = fixedAssetStatus.Id;
            try
            {
                _context.Add(fixedAsset);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Fixed Asset created successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Error creating asset " + ex.Message;
                ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetCategories"), "Id", "Description", fixedAsset.CategoryId);
                ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", fixedAsset.ResponsibleEmployeeId);
                ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetStatus"), "Id", "Description", fixedAsset.StatusId);
                return View(fixedAsset);
            }
           
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
            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetCategories"), "Id", "Description", fixedAsset.CategoryId);
            ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", fixedAsset.ResponsibleEmployeeId);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetStatus"), "Id", "Description", fixedAsset.StatusId);
            return View(fixedAsset);
        }

        // POST: FixedAssets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  FixedAsset fixedAsset)
        {
            var userId = User.GetUserId();
            if (id != fixedAsset.Id)
            {
                return NotFound();
            }
            if (!FixedAssetExists(fixedAsset.Id))
            {
                return NotFound();
            }
            
                try
                {
                fixedAsset.ModifiedById = User.GetUserName();
                fixedAsset.ModifiedOn = DateTime.Now;
                    _context.Update(fixedAsset);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Fixed Asset updated successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
                {
                    TempData["Error"] = "Error updated asset " + ex.Message;
                ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetCategories"), "Id", "Description", fixedAsset.CategoryId);
                ViewData["ResponsibleEmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", fixedAsset.ResponsibleEmployeeId);
                ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(x => x.SystemCodeValue.Code == "AssetStatus"), "Id", "Description", fixedAsset.StatusId);
                return View(fixedAsset);
            }
            
           
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
            var userId = User.GetUserId();
            if (fixedAsset != null)
            {
                _context.FixedAssets.Remove(fixedAsset);
            }
            try
            {
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Fixed Asset deleted successfully ";
                return RedirectToAction(nameof(Index));
            }
           

           
             catch (Exception ex)
            {
                TempData["Error"] = "Error delete asset " + ex.Message;
                return View(fixedAsset);
            }
        }

        private bool FixedAssetExists(int id)
        {
            return _context.FixedAssets.Any(e => e.Id == id);
        }
    }
}
