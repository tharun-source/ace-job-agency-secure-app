using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
public string Email { get; set; }

      [Required(ErrorMessage = "OTP code is required")]
     [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
     public string Otp { get; set; }
    }
}
