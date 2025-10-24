using DocumentFormat.OpenXml.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesManagment.Models
{
    public class Salary : ApprovalActivity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        // مكونات الراتب
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Allowances { get; set; } = 0;
   

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; } = 0;
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; } = 0;
        [Column(TypeName = "varchar(10)")]
        public string? Currency {  get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? SalariesNotes { get; set; }
        public int? BankId { get; set; }
        public Bank Bank { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IBAN { get; set; }
        public string? SWIFTCode { get; set; }

        public string? NSSFNO { get; set; }
        public string? NHIF { get; set; }
        public bool PaysTax { get; set; }
        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
        public string? ApprovalNotes { get; set; }

    }
}
