using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
using EmployeesManagment.Hubs;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeesManagment.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notifications
        // GET: Notificationsل 
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();

            return View(notifications);
        }

        public async Task<IActionResult> NotIsRead()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();

            return View(notifications);
        }

        public async Task<IActionResult> Unread()
        {
            var userId = _userManager.GetUserId(User);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();

            return View(notifications);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetUserNotifications()
        //{
        //    var userId = _userManager.GetUserId(User);

        //    var notifications = await _context.Notifications
        //        .Where(n => n.UserId == userId)
        //        .OrderByDescending(n => n.CreatedOn)
        //        .Take(5)
        //        .Select(n => new
        //        {
        //            n.Id,
        //            n.Message,
        //            n.Url,
        //            n.IsRead,
        //            CreatedOn = n.CreatedOn.ToString("g")
        //        })
        //        .ToListAsync();

        //    var unreadCount = await _context.Notifications
        //        .CountAsync(n => n.UserId == userId && !n.IsRead);

        //    return Json(new { notifications, unreadCount });
        //}
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .Take(20)
                .ToListAsync();

            var unreadCount = notifications.Count(n => !n.IsRead);

            return Json(new
            {
                unreadCount,
                notifications = notifications.Select(n => new {
                    n.Id,
                    n.Message,
                    n.Url,
                    n.IsRead,
                    CreatedOn = n.CreatedOn.ToString("yyyy-MM-dd HH:mm")
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync(userId);

            return Ok();
        }


        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification notification)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                notification.CreatedOn = DateTime.Now;
                notification.CreatedById = User.GetUserName();
                _context.Add(notification);
                await _context.SaveChangesAsync(userId);
                return RedirectToAction(nameof(Index));
            }
               
            catch(Exception ex)
            {
                return View(notification);
            }
            
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Notification notification)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != notification.Id)
            {
                return NotFound();
            }
            if (!NotificationExists(notification.Id))
            {
                return NotFound();
            }

            try
                {
                notification.ModifiedOn = DateTime.Now;
                notification.ModifiedById = User.GetUserName();
                notification.Message = "Done";
                    _context.Update(notification);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "updated notification successfully ";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error updating notification " + ex.Message +
                                    (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");
                return View(notification);
            }



        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
