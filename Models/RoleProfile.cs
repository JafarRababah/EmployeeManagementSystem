using Microsoft.AspNetCore.Identity;

namespace EmployeesManagment.Models
{
    public class RoleProfile
    {
        public int Id { get; set; }

        public int TaskId { get; set; }           // ✅ نوعه int ليطابق SystemProfile.Id
        public SystemProfile Task { get; set; }   // ✅ المرجع الملازم

        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }
    }
}
