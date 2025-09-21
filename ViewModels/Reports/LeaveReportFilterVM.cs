using EmployeesManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EmployeesManagment.ViewModels.Reports
{
    public class LeaveReportFilterVM
    {
        public string? EmployeeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<LeaveApplication> LeaveApplications { get; set; }
    }
}
