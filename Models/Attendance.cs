using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesManagment.Models
{
    public class Attendance : UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        // حساب الساعات الإضافية
        public decimal OvertimeHours { get; set; } = 0;

        // التأخير بالدقائق
        public int LateMinutes { get; set; } = 0;

        // حالة الحضور (حاضر، غائب، إجازة)
        public string Status { get; set; } = "Present";
    }
}
