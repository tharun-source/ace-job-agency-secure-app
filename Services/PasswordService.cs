using System;
using System.Linq;
using System.Text.RegularExpressions;
using BCrypt.Net;

namespace Application_Security_Asgnt_wk12.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public bool ValidatePasswordStrength(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password is required.";
                return false;
            }

            if (password.Length < 12)
            {
                errorMessage = "Password must be at least 12 characters long.";
                return false;
            }

            if (!password.Any(char.IsLower))
            {
                errorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }

            if (!password.Any(char.IsUpper))
            {
                errorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (!password.Any(char.IsDigit))
            {
                errorMessage = "Password must contain at least one number.";
                return false;
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
            {
                errorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }

        public bool CheckPasswordHistory(string newPasswordHash, string? passwordHistory, int maxHistoryCount = 2)
        {
            if (string.IsNullOrEmpty(passwordHistory))
                return true;

            var historicalHashes = passwordHistory.Split('|', StringSplitOptions.RemoveEmptyEntries);

            // Check if new password matches any in history
            foreach (var oldHash in historicalHashes.Take(maxHistoryCount))
            {
                // Since we can't reverse hash, we'll compare hashes directly
                if (oldHash == newPasswordHash)
                    return false;
            }

            return true;
        }

        public string AddToPasswordHistory(string currentHash, string? existingHistory, int maxHistoryCount = 2)
        {
            var historyList = string.IsNullOrEmpty(existingHistory)
                ? new List<string>()
                : existingHistory.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

            historyList.Insert(0, currentHash);

            // Keep only the specified number of historical passwords
            if (historyList.Count > maxHistoryCount)
            {
                historyList = historyList.Take(maxHistoryCount).ToList();
            }

            return string.Join("|", historyList);
        }

        // Password Reset Methods
        public string GeneratePasswordResetToken()
        {
            // Generate a secure random token
            return Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
        }

        public bool IsResetTokenValid(DateTime? tokenExpiry)
        {
            if (!tokenExpiry.HasValue)
                return false;

            // Token is valid if it hasn't expired (15 minutes validity)
            return tokenExpiry.Value > DateTime.UtcNow;
        }

        public DateTime GetResetTokenExpiry()
        {
            // Token expires in 15 minutes
            return DateTime.UtcNow.AddMinutes(15);
        }

        // Password Age Policy Methods
        public const int MinimumPasswordAgeDays = 1; // Can't change password within 1 day
        public const int MaximumPasswordAgeDays = 90; // Must change password after 90 days
        public const int PasswordExpirationWarningDays = 10; // Warn when 10 days left

        public bool CanChangePassword(DateTime? lastPasswordChangedDate, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!lastPasswordChangedDate.HasValue)
            {
                // First time password change or no record - allow it
                return true;
            }

            var daysSinceLastChange = (DateTime.UtcNow - lastPasswordChangedDate.Value).TotalDays;

            if (daysSinceLastChange < MinimumPasswordAgeDays)
            {
                var hoursRemaining = (MinimumPasswordAgeDays * 24) - (daysSinceLastChange * 24);
                errorMessage = $"You must wait at least {MinimumPasswordAgeDays} day(s) before changing your password again. Time remaining: {Math.Ceiling(hoursRemaining)} hours.";
                return false;
            }

            return true;
        }

        public PasswordAgeStatus GetPasswordAgeStatus(DateTime? lastPasswordChangedDate)
        {
            if (!lastPasswordChangedDate.HasValue)
            {
                return new PasswordAgeStatus
                {
                    IsExpired = false,
                    DaysUntilExpiration = MaximumPasswordAgeDays,
                    DaysSinceLastChange = 0,
                    ShouldWarn = false,
                    CanChange = true
                };
            }

            var daysSinceLastChange = (DateTime.UtcNow - lastPasswordChangedDate.Value).TotalDays;
            var daysUntilExpiration = MaximumPasswordAgeDays - daysSinceLastChange;
            var isExpired = daysSinceLastChange >= MaximumPasswordAgeDays;
            var shouldWarn = daysUntilExpiration <= PasswordExpirationWarningDays && daysUntilExpiration > 0;
            var canChange = daysSinceLastChange >= MinimumPasswordAgeDays;

            return new PasswordAgeStatus
            {
                IsExpired = isExpired,
                DaysUntilExpiration = Math.Max(0, (int)Math.Ceiling(daysUntilExpiration)),
                DaysSinceLastChange = (int)Math.Floor(daysSinceLastChange),
                ShouldWarn = shouldWarn,
                CanChange = canChange,
                HoursUntilCanChange = canChange ? 0 : (int)Math.Ceiling((MinimumPasswordAgeDays * 24) - (daysSinceLastChange * 24))
            };
        }
    }

    public class PasswordAgeStatus
    {
        public bool IsExpired { get; set; }
        public int DaysUntilExpiration { get; set; }
        public int DaysSinceLastChange { get; set; }
        public bool ShouldWarn { get; set; }
        public bool CanChange { get; set; }
        public int HoursUntilCanChange { get; set; }
    }
}
