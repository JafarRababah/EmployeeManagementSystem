using Microsoft.AspNetCore.Identity;

namespace EmployeesManagment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public string? NationalId { get; set; }

        public string? CreatedById { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? ModifiedById { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string? RoleId { get; set; } // مجرد تخزين ID اختياري
        public IdentityRole? Role { get; set; } // Navigation property (غير ضروري غالبًا)

        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim();
    }
}
