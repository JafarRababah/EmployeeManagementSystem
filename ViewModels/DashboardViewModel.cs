namespace EmployeesManagment.ViewModels
{
    public class DashboardViewModel
    {
        public int PendingLeaves { get; set; }
        public int NewEmployees { get; set; }
        public int LateRate { get; set; }
        public int TotalSalary { get; set; }
        // بيانات الرسم البياني
        public List<string> Months { get; set; } = new();
        public List<int> AttendanceData { get; set; } = new();
        public List<int> LeaveData { get; set; } = new();
    }

}
