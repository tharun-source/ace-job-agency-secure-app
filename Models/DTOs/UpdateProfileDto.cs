using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
      public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

   [Required]
        public DateTime DateOfBirth { get; set; }

   public string? WhoAmI { get; set; }

      public IFormFile? Resume { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
