using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagment.Models
{
    public class LeaveAdjustmentEntry : UserActivity
    {
        public int Id { get; set; }

        [Display(Name = "Leave Period")]
        public int? LeavePeriodId { get; set; }
        public LeavePeriod LeavePeriod { get; set; }

        [Required(ErrorMessage = "Please select an employee")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Display(Name = "No. of Days")]
        public decimal NoOfDays { get; set; }

        [Display(Name = "Adjustment Date")]
        public DateTime LeaveAdjusmentDate { get; set; }

        [Display(Name = "Leave Start Date")]
        public DateTime? LeaveStartDate { get; set; }

        [Display(Name = "Leave End Date")]
        public DateTime? LeaveEndDate { get; set; }

        [Display(Name = "Description")]
        public string AdjustmentDescription { get; set; }

        [Required(ErrorMessage = "Please select adjustment type")]
        [Display(Name = "Adjustment Type")]
        public int AdjustmentTypeId { get; set; }
        public SystemCodeDetail AdjustmentType { get; set; }
    }
}
