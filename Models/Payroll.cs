using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesManagment.Models
{
    public class Payroll:UserActivity
    {
        [Key]
        public int PayrollId { get; set; }

        // ربط بالموظف (AspNetUsers)
        [Required]
        public int EmployeeId { get; set; }  

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }  

        // فترة الراتب
        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        // مكونات الراتب
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Allowances { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Overtime { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }
        public Payroll()
        {
            var today = DateTime.Today;
            PeriodStart = new DateTime(today.Year, today.Month, 1); // أول يوم بالشهر
            PeriodEnd = PeriodStart.AddMonths(1).AddDays(-1);      // آخر يوم بالشهر
        }

    }
}
