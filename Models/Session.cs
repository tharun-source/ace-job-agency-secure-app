using System;
using System.ComponentModel.DataAnnotations;


namespace Application_Security_Asgnt_wk12.Models
{
    public class UserSession
    {
        [Key]   
        public int Id { get; set; }
        public int MemberId { get; set; }
        [Required]
        public string SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public bool IsActive { get; set; }
    }
}
