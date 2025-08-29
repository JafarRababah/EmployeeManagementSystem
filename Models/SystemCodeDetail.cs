using System.ComponentModel.DataAnnotations;

namespace EmployeesManagment.Models
{
    public class SystemCodeDetail:UserActivity
    {
        [Key]
        public int Id { get; set; }
        public int SystemCodeId { get; set; }
        public SystemCode SystemCodeValue { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? OrderNo { get; set; }
    }
}
