using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeesManagment.Models
{
    public class License
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseKey { get; set; } // مفتاح الترخيص
        [Required]
        public string LicenseHash { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; } // تاريخ انتهاء الترخيص

        public bool IsActive { get; set; } = true; // حالة الترخيص (فعال/منتهي)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ إنشاء الترخيص
   
    }
}
