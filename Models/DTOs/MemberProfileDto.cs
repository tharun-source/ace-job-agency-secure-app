namespace Application_Security_Asgnt_wk12.Models.DTOs
{
    public class MemberProfileDto
    {
        public int Id { get; set; }
public string FirstName { get; set; }
      public string LastName { get; set; }
        public string Gender { get; set; }
      public string NRICDecrypted { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
 public string? ResumePath { get; set; }
    public string? WhoAmI { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
