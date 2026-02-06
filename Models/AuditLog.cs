using System;
using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        public string MemberId { get; set; }
        [Required]
        public string Action { get; set; }
        
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }  // Make nullable
    }
}
