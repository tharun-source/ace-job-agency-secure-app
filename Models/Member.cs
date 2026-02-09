using System;
using System.ComponentModel.DataAnnotations;

namespace Application_Security_Asgnt_wk12.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string NRICEncrypted { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string ResumePath { get; set; }

        public string? WhoAmI { get; set; }

        public string? PhotoPath { get; set; }
        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedOutUntil { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string? PasswordHistory { get; set; }

        // Password Reset Fields
        public string? PasswordResetToken { get; set; }
        
        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Two-Factor Authentication Fields
        public bool IsTwoFactorEnabled { get; set; } = false;
        public string? CurrentOtp { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public string? BackupCodes { get; set; } // Encrypted, comma-separated
    }
}
