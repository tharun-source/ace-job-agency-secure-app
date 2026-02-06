using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
