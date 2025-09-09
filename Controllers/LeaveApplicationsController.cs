using EmployeesManagment.Data;
using EmployeesManagment.Data.Migrations;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace EmployeesManagment.Controllers
{
    public class LeaveApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly NotificationService _notificationService;
        private readonly ILogger<LeaveApplicationsController> _logger;


        public LeaveApplicationsController(ApplicationDbContext context, IConfiguration configuration, NotificationService notificationService, ILogger<LeaveApplicationsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;
            _logger = logger;
        }


        // GET: LeaveApplications
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var awaitingStatus = _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus" && y.Code == "AwaitingApproval" ).FirstOrDefault();
            var leaveapplication = _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status).OrderByDescending(l => l.CreatedOn);
                
            return View(leaveapplication);
        }
        public async Task<IActionResult> ApprovedLeaveApplications()
        {
            var approvedStatus = _context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus" && y.Code == "Approved").FirstOrDefault();

            var applicationDbContext = _context.LeaveApplications.
                Include(l => l.Duration).
                Include(l => l.Employee).
                Include(l => l.LeaveType).
                Include(l => l.Status).
                Where(l=>l.StatusId==approvedStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> RejectedLeaveApplications()
        {
            var rejectedStatus = _context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefault();

            var applicationDbContext = _context.LeaveApplications.
                Include(l => l.Duration).
                Include(l => l.Employee).
                Include(l => l.LeaveType).
                Include(l => l.Status).
                Where(l => l.StatusId == rejectedStatus!.Id);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: LeaveApplications/Details/5
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // GET: LeaveApplications/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCodeValue).Where(y=>y.SystemCodeValue.Code=="LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ApprovedLeave(int? id)
        {
            var leaveApplication=await _context.LeaveApplications
                .Include(l=>l.Duration)
                .Include(l=>l.Employee)
                .Include(l=>l.LeaveType)
                .Include(l=>l.Status)
                .FirstOrDefaultAsync(m=>m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
           return View(leaveApplication);

        }

        [HttpPost]
        public async Task<IActionResult> ApprovedLeave(LeaveApplication leave)
        {
            try
            {
                var approvedStatus = _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus" && y.Code == "Approved").FirstOrDefault();
                var adjustmentType = _context.SystemCodeDetails
                    .Include(x => x.SystemCodeValue)
                    .Where(y => y.SystemCodeValue.Code == "LeaveAdjustment" && y.Code == "Negative").FirstOrDefault();
                var leaveApplication = await _context.LeaveApplications
                    .Include(l => l.Duration)
                    .Include(l => l.Employee)
                    .Include(l => l.LeaveType)
                    .Include(l => l.Status)
                    .FirstOrDefaultAsync(m => m.Id == leave.Id);
                if (leaveApplication == null)
                {
                    return NotFound();
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                leaveApplication.ApprovedOn = DateTime.Now;
                leaveApplication.ApprovedById = userId;
                leaveApplication.StatusId = approvedStatus!.Id;
                leaveApplication.ApprovalNotes = leave.ApprovalNotes;
                _context.Update(leaveApplication);
                await _context.SaveChangesAsync(userId);
                var adjustment = new LeaveAdjustmentEntry
                {
                    EmployeeId = leaveApplication.EmployeeId,
                    NoOfDays = leaveApplication.NoOfDays,
                    LeaveStartDate = leaveApplication.StartDate,
                    LeaveEndDate = leaveApplication.EndDate,
                    AdjustmentDescription = "Leave Taken-Negative Adjusment",
                    LeavePeriodId = 1,
                    LeaveAdjusmentDate = DateTime.Now,
                    AdjustmentTypeId = adjustmentType.Id
                };
                _context.Add(adjustment);
                await _context.SaveChangesAsync(userId);
                var employee = await _context.Employees.FindAsync(leaveApplication.EmployeeId);
                employee.LeaveOutStandingBalance = employee.AllocatedLeaveDays - leaveApplication.NoOfDays;
                _context.Update(employee);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Leave application approved successfully ";
                
                return RedirectToAction(nameof(Index));
            }catch(Exception ex)
            {
                TempData["Error"] = "Leave application Not approved by successfully ";
                ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.
                    Include(x => x.SystemCodeValue).
                    Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description");
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
                return View(leave);
            }
            
        }
        [HttpGet]
            public async Task<IActionResult> RejectLeave(int? id)
            {
            
                var leaveApplication = await _context.LeaveApplications
                    .Include(l => l.Duration)
                    .Include(l => l.Employee)
                    .Include(l => l.LeaveType)
                    .Include(l => l.Status)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (leaveApplication == null)
                {
                    return NotFound();
                }
                ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description",leaveApplication.DurationId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
                return View(leaveApplication);
      
            
        }
        [HttpPost]
        public async Task<IActionResult> RejectLeave(LeaveApplication leave)
        {
            try
            {
                var rejectedStatus = _context.SystemCodeDetails
                    .Include(x => x.SystemCodeValue)
                    .Where(y => y.SystemCodeValue.Code == "LeaveApprovalStatus" && y.Code == "Rejected").FirstOrDefault();
                var leaveApplication = await _context.LeaveApplications
                    .Include(l => l.Duration)
                    .Include(l => l.Employee)
                    .Include(l => l.LeaveType)
                    .Include(l => l.Status)
                    .FirstOrDefaultAsync(m => m.Id == leave.Id);
                if (leaveApplication == null)
                {
                    return NotFound();
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                leaveApplication.ApprovedOn = DateTime.Now;
                leaveApplication.ApprovedById = userId;
                leaveApplication.StatusId = rejectedStatus.Id;
                leaveApplication.ApprovalNotes = leave.ApprovalNotes;
                _context.Update(leaveApplication);
                await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Leave application rejected successfully ";
               
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Leave application not rejected by successfully ";
                ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue).Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description", leave.DurationId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName");
                ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name");
                return View(leave);
            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveApplication leaveApplication, IFormFile leaveAttachment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // --- التحقق من الحقول المطلوبة ---
            if (leaveApplication.EmployeeId == 0)
                ModelState.AddModelError("EmployeeId", "Employee is required.");

            if (leaveApplication.LeaveTypeId == 0)
                ModelState.AddModelError("LeaveTypeId", "Leave type is required.");

            
            try
            {
                // --- حفظ المرفق إذا موجود ---
                if (leaveAttachment != null && leaveAttachment.Length > 0)
                {
                    var fileName = "LeaveAttachment_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + leaveAttachment.FileName;
                    var path = _configuration["FileSettings:UploadFolder"];
                    var filePath = Path.Combine(path, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await leaveAttachment.CopyToAsync(stream);

                    leaveApplication.Attachment = fileName;
                }

                // --- ضبط التواريخ والحالة ---
                leaveApplication.EndDate = leaveApplication.StartDate.AddDays(leaveApplication.NoOfDays - 1);
                leaveApplication.CreatedOn = DateTime.Now;
                leaveApplication.CreatedById = userId;

                var pendingStatus = await _context.SystemCodeDetails
                    .Include(x => x.SystemCodeValue)
                    .FirstOrDefaultAsync(y => y.Code == "AwaitingApproval" && y.SystemCodeValue.Code == "LeaveApprovalStatus");

                if (pendingStatus == null)
                {
                    ModelState.AddModelError("", "Status 'AwaitingApproval' not found.");
                    return View(leaveApplication);
                }

                leaveApplication.StatusId = pendingStatus.Id;

                // --- حفظ طلب الإجازة ---
                _context.LeaveApplications.Add(leaveApplication);
                await _context.SaveChangesAsync(userId);

                // --- إعداد الموافقات ---
                var documentType = await _context.SystemCodeDetails
                    .Include(x => x.SystemCodeValue)
                    .FirstOrDefaultAsync(x => x.SystemCodeValue.Code == "DocumentTypes" && x.Code == "LeaveApplication");

                var userGroup = await _context.ApprovalsUserMatrixes
                    .FirstOrDefaultAsync(x => x.DocumentTypeId == documentType.Id && x.Active);

                var approvers = await _context.WorkFlowUserGroupMembers
                    .Where(x => x.WorkFlowUserGroupId == userGroup.workFlowUserGroupId)
                    .ToListAsync();

                foreach (var approver in approvers)
                {
                    var approvalEntry = new ApprovalEntry
                    {
                        ApproverId = approver.ApproverId,
                        RecordId = leaveApplication.Id,
                        ControllerName = "LeaveApplications",
                        DocumentTypeId = documentType.Id,
                        DateSentForApproval = DateTime.Now,
                        LastModifiedOn = DateTime.Now,
                        LastModifiedId = userId,
                        SequenceNo = approver.SequenceNo,
                        StatusId = pendingStatus.Id,
                        Comments = "Sent For Approval"
                    };
                    _context.ApprovalEntries.Add(approvalEntry);
                }

                await _context.SaveChangesAsync(userId);

                // --- إرسال إشعار للمدير/الآدمن ---
                var managerId = await _context.Users
                    .Where(u => u.RoleId == "Admin")
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(managerId))
                {
                    await _notificationService.AddNotificationAsync(
                        managerId,
                        $"📢 طلب إجازة جديد من {leaveApplication.Employee?.FullName ?? "موظف"}",
                        Url.Action("Details", "LeaveApplications", new { id = leaveApplication.Id })
                    );
                }

                TempData["Message"] = "Leave application created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave application.");
                TempData["Error"] = "Error creating leave application: " + (ex.InnerException?.Message ?? ex.Message);

                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
                ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
                ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue)
                    .Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description", leaveApplication.DurationId);
                return View(leaveApplication);
            }
        }


        // POST: LeaveApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(LeaveApplication leaveApplication, IFormFile leaveattachment)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    // Validate required fields
        //    if (leaveApplication.EmployeeId == 0)
        //        ModelState.AddModelError("EmployeeId", "Employee is required.");

        //    if (leaveApplication.LeaveTypeId == 0)
        //        ModelState.AddModelError("LeaveTypeId", "Leave type is required.");


        //    try
        //    {
        //        // Handle file attachment
        //        if (leaveattachment != null && leaveattachment.Length > 0)
        //        {
        //            var fileName = "LeaveAttachment_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + leaveattachment.FileName;
        //            var path = _configuration["FileSettings:UploadFolder"];
        //            var filePath = Path.Combine(path, fileName);

        //            await using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await leaveattachment.CopyToAsync(stream);
        //            }

        //            leaveApplication.Attachment = fileName;
        //        }

        //        // Calculate end date
        //        leaveApplication.EndDate = leaveApplication.StartDate.AddDays(leaveApplication.NoOfDays - 1);
        //        leaveApplication.CreatedOn = DateTime.Now;
        //        leaveApplication.CreatedById = userId;

        //        // Get pending status
        //        var pendingStatus = await _context.SystemCodeDetails
        //            .Include(x => x.SystemCodeValue)
        //            .FirstOrDefaultAsync(y => y.Code == "AwaitingApproval" && y.SystemCodeValue.Code == "LeaveApprovalStatus");

        //        if (pendingStatus == null)
        //        {
        //            ModelState.AddModelError("", "Status 'AwaitingApproval' not found.");
        //            return View(leaveApplication);
        //        }

        //        leaveApplication.StatusId = pendingStatus.Id;

        //        // Wrap in transaction
        //        using var transaction = await _context.Database.BeginTransactionAsync();

        //        // Save leave application
        //        _context.Add(leaveApplication);
        //        await _context.SaveChangesAsync(userId);

        //        // Prepare workflow
        //        var documentType = await _context.SystemCodeDetails
        //            .Include(x => x.SystemCodeValue)
        //            .FirstOrDefaultAsync(x => x.SystemCodeValue.Code == "DocumentTypes" && x.Code == "LeaveApplication");

        //        var userGroup = await _context.ApprovalsUserMatrixes
        //            .FirstOrDefaultAsync(x => x.DocumentTypeId == documentType.Id && x.Active);

        //        var approvers = await _context.WorkFlowUserGroupMembers
        //            .Where(x => x.WorkFlowUserGroupId == userGroup.workFlowUserGroupId)
        //            .ToListAsync();

        //        var waitingApproval = pendingStatus; // reuse pending status

        //        // Notify admin / manager
        //        var managerId = await _context.Users
        //            .Where(u => u.RoleId == "Admin") // adjust per your roles
        //            .Select(u => u.Id)
        //            .FirstOrDefaultAsync();

        //        if (!string.IsNullOrEmpty(managerId))
        //        {
        //            try
        //            {
        //                await _notificationService.AddNotificationAsync(
        //                    managerId,
        //                    $"📢 طلب إجازة جديد من {leaveApplication.Employee?.FullName ?? "موظف"}",
        //                    Url.Action("Details", "LeaveApplications", new { id = leaveApplication.Id })
        //                );
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Failed to send notification to manager.");
        //            }
        //        }

        //        // Add approval entries
        //        foreach (var approver in approvers)
        //        {
        //            var approvalEntry = new ApprovalEntry
        //            {
        //                ApproverId = approver.ApproverId,
        //                DateSentForApproval = DateTime.Now,
        //                LastModifiedOn = DateTime.Now,
        //                LastModifiedId = approver.SenderId,
        //                RecordId = leaveApplication.Id,
        //                ControllerName = "LeaveApplications",
        //                DocumentTypeId = documentType.Id,
        //                SequenceNo = approver.SequenceNo,
        //                StatusId = waitingApproval.Id,
        //                Comments = "Sent For Approval"
        //            };

        //            _context.Add(approvalEntry);
        //        }

        //        await _context.SaveChangesAsync(userId);
        //        await transaction.CommitAsync();

        //        TempData["Message"] = "Leave application created successfully.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating leave application.");
        //        TempData["Error"] = "Error creating Leave application: " + (ex.InnerException?.Message ?? ex.Message);

        //        ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
        //        ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
        //        ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCodeValue)
        //            .Where(y => y.SystemCodeValue.Code == "LeaveDuration"), "Id", "Description", leaveApplication.DurationId);

        //        return View(leaveApplication);
        //    }
        //}




        // GET: LeaveApplications/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.Code == "Pending" && y.SystemCodeValue.Code == "LeaveApprovalStatus")
                .FirstOrDefaultAsync();

            if (pendingStatus == null)
            {
                ModelState.AddModelError("", "Status 'Pending' not found.");
                // عرض صفحة الخطأ أو رسالة مناسبة
                return View(leaveApplication); // في هذه الحالة leaveApplication ليس null
            }

            ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveApplication.DurationId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
            ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
            return View(leaveApplication);
        }


        // POST: LeaveApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveApplication leaveApplication,IFormFile leaveattachment)
        {
            try
            {
                if (leaveattachment != null && leaveattachment.Length > 0)
            {
                var fileName = "LeaveAttachment_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + leaveattachment.FileName;
                var path = _configuration["FileSettings:UploadFolder"];
                var filepath = Path.Combine(path, fileName);
                await using (var stream = new FileStream(filepath, FileMode.Create))
                {
                    await leaveattachment.CopyToAsync(stream);
                }
                leaveApplication.Attachment = fileName;
            }
            if (id != leaveApplication.Id)
            {
                return NotFound();
            }
            if (!LeaveApplicationExists(leaveApplication.Id))
            {
                return NotFound();
            }
            var pendingStatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCodeValue)
                .Where(y => y.Code == "Pending" && y.SystemCodeValue.Code == "LeaveApprovalStatus")
                .FirstOrDefaultAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (pendingStatus == null)
            {
                ModelState.AddModelError("", "Status 'Pending' not found.");
                return View(leaveApplication);
            }

            
               
                    leaveApplication.ModifiedOn = DateTime.Now;
                    leaveApplication.ModifiedById = userId;
                leaveApplication.ApprovedOn = DateTime.Now;
                leaveApplication.ApprovedById = userId;
                leaveApplication.StatusId = pendingStatus.Id;

                    _context.Update(leaveApplication);
                    await _context.SaveChangesAsync(userId);
                TempData["Message"] = "Leave application created successfully ";
                return RedirectToAction(nameof(Index));
            }
                catch (Exception ex)
                {
                TempData["Error"] = "Error creating Leave application " + ex.Message;
                ViewData["DurationId"] = new SelectList(_context.SystemCodeDetails, "Id", "Description", leaveApplication.DurationId);
                ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FullName", leaveApplication.EmployeeId);
                ViewData["LeaveTypeId"] = new SelectList(_context.LeaveTypes, "Id", "Name", leaveApplication.LeaveTypeId);
                return View(leaveApplication);
            }
                
            

           
        }


        // GET: LeaveApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Duration)
                .Include(l => l.Employee)
                .Include(l => l.LeaveType)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // POST: LeaveApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            if (leaveApplication != null)
            {
                _context.LeaveApplications.Remove(leaveApplication);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications.Any(e => e.Id == id);
        }
    }
}
