using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class ResetPasswordDto
    {
        [Required]
   public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    [Required]
   public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
