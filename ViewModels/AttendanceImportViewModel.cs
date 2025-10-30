using System.ComponentModel.DataAnnotations;

namespace EmployeesManagment.ViewModels
{
    public class AttendanceImportViewModel
    {
        [Required]
        [Display(Name = "ملف البصمة (CSV)")]
        public IFormFile CsvFile { get; set; }
    }

}
