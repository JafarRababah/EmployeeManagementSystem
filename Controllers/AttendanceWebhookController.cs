using EmployeesManagment.Data;
using EmployeesManagment.Models;
using EmployeesManagment.Services;
using EmployeesManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendanceWebhookController> _logger;

        public AttendanceWebhookController(ApplicationDbContext context, ILogger<AttendanceWebhookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveAttendance([FromBody] AttendanceWebhookDto dto)
        {
            try
            {
                if (dto == null || dto.EmployeeId==null)
                    return BadRequest("Invalid payload");

                // البحث عن الموظف
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
                var userId = "ca08e19b-b584-4a9a-b5f6-cff426a1be91";
                if (employee == null)
                {
                    _logger.LogWarning($"Employee not found for code: {dto.EmployeeId}");
                    return NotFound($"Employee not found for code {dto.EmployeeId}");
                }

                // تحديد حالة الحضور
                var status = await _context.SystemCodeDetails
                    .FirstOrDefaultAsync(s => s.Code == dto.Status);

                // التحقق من وجود سجل بنفس التاريخ
                var existingRecord = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date.Date == dto.PunchTime.Date);

                if (existingRecord == null)
                {
                    // إنشاء سجل جديد
                    var attendance = new Attendance
                    {
                        EmployeeId = employee.Id,
                        Date = dto.PunchTime.Date,
                        CheckIn = dto.Status == "CheckIn" ? dto.PunchTime : null,
                        CheckOut = dto.Status == "CheckOut" ? dto.PunchTime : null,
                        TotalHours = 0,
                        OvertimeHours = 0,
                        LateMinutes = 0,
                        CreatedOn=DateTime.Now,
                        ModifiedOn=DateTime.Now,
                        StatusId = status?.Id ?? 0,
                        Source = "DeviceWebhook"
                    };

                    _context.Attendances.Add(attendance);
                }
                else
                {
                    // تحديث السجل الموجود
                    if (dto.Status == "CheckIn")
                        existingRecord.CheckIn = dto.PunchTime;
                    else if (dto.Status == "CheckOut")
                        existingRecord.CheckOut = dto.PunchTime;

                    existingRecord.StatusId = status?.Id ?? existingRecord.StatusId;
                    existingRecord.Source = "DeviceWebhook";
                    _context.Attendances.Update(existingRecord);
                }

                await _context.SaveChangesAsync(userId);

                return Ok(new { message = "Attendance record saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook data");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    // DTO لاستقبال البيانات من جهاز البصمة
    public class AttendanceWebhookDto
    {
        public int EmployeeId { get; set; }
        public DateTime PunchTime { get; set; }
        public int OverTimeHours { get; set; }
        public int LateMinutes { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string Status { get; set; }
    }
}
