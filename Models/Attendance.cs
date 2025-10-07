using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesManagment.Models
{
    public class Attendance : UserActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        
        public Employee Employee { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Check In")]
        [DataType(DataType.Time)]
        public DateTime? CheckIn { get; set; }
        [Display(Name = "Check Out")]
        [DataType(DataType.Time)]
        public DateTime? CheckOut { get; set; }
        public double TotalHours { get; set; }

        // حساب الساعات الإضافية
        [Display(Name = "Over Time")]
        public decimal OvertimeHours { get; set; } = 0;

        // التأخير بالدقائق
        [Display(Name = "Late Minutes")]
        public int LateMinutes { get; set; } = 0;

        // حالة الحضور (حاضر، غائب، إجازة)
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
    }
}
