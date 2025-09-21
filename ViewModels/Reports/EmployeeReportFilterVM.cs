using EmployeesManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EmployeesManagment.ViewModels.Reports
{
    public class EmployeeReportFilterVM
    {
        public string? FullName { get; set; }
        public int? DepartmentId { get; set; }

        // لعرض النتائج
        public IEnumerable<Employee> Employees { get; set; }

        // للقائمة المنسدلة
        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}
