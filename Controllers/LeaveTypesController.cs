using AutoMapper;
using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
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
    public class LeaveTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;
        private readonly IExtensionService _extesionService;
        public LeaveTypesController(ApplicationDbContext context, IMapper mapper, IExtensionService extensionService)
        {
            _context = context;
            _mapper = mapper;
            _extesionService = extensionService;
        }

        // GET: LeaveTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveTypes.ToListAsync());
        }

        // GET: LeaveTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        // GET: LeaveTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveType leaveType)
        {
            try
            {
                leaveType.Code = await _extesionService.GenerateLeaveTypeNumber();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    leaveType.CreatedOn = DateTime.Now;
                    leaveType.CreatedById = User.Identity.Name;
                    leaveType.ModifiedById= User.Identity.Name;
                    leaveType.ModifiedOn = DateTime.Now;
                    _context.Add(leaveType);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Leave type created successfully ";
                    return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error creating Leave Type " + ex.Message;
                return View(leaveType);
            }
        }

        // GET: LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,LeaveType leaveType)
        {
            if (id != leaveType.Id)
            {
                return NotFound();
            }
            if (!LeaveTypeExists(leaveType.Id))
            {
                return NotFound();
            }

            try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var oldLeaveType = await _context.LeaveTypes.FindAsync(id);
                    leaveType.ModifiedById = User.Identity.Name;
                    leaveType.ModifiedOn = DateTime.Now;
                    _context.Entry(oldLeaveType).CurrentValues.SetValues(leaveType);
                    await _context.SaveChangesAsync(userId);
                    TempData["Message"] = "Leave type updated successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
                {
                   
                TempData["Error"] = "Error update Leave type " + ex.Message;
                return View(leaveType);

            }

        }

        // GET: LeaveTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType != null)
            {
                _context.LeaveTypes.Remove(leaveType);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error delete leave type" + ex.Message;
                return View(leaveType);
            }
        }

        private bool LeaveTypeExists(int id)
        {
            return _context.LeaveTypes.Any(e => e.Id == id);
        }
    }
}
