using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First name is required")]
  [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
     [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name can only contain letters and spaces")]
   public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
     [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name can only contain letters and spaces")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "NRIC is required")]
        [RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Invalid NRIC format")]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Email is required")]
   [EmailAddress(ErrorMessage = "Invalid email format")]
      [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

   [Required(ErrorMessage = "Please confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
      public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

   [Required(ErrorMessage = "Resume is required")]
        public IFormFile Resume { get; set; }

        [StringLength(1000, ErrorMessage = "Who Am I cannot exceed 1000 characters")]
        public string? WhoAmI { get; set; }

        public IFormFile? Photo { get; set; }

        [Required(ErrorMessage = "Please complete the CAPTCHA")]
 public string CaptchaToken { get; set; }
    }
}
