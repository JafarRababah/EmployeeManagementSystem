using System;

namespace EmployeesManagment.ViewModels.Reports
{
    public class LeaveReportRow
    {
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
