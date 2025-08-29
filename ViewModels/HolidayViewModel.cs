using EmployeesManagment.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagment.ViewModels
{
    public class HolidayViewModel:UserActivity
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public Holiday Holiday { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
}
