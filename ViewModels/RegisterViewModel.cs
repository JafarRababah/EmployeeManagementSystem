using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }
    public string NationalId { get; set; }
    public string PhoneNumber { get; set; }
    [Required, EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required, DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; }

    [Required, DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }
}
