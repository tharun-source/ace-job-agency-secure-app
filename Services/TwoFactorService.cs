using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application_Security_Asgnt_wk12.Services
{
  public class TwoFactorService
    {
private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
   private readonly EncryptionService _encryptionService;
        private readonly ILogger<TwoFactorService> _logger;
        private const int OtpExpiryMinutes = 5;

        public TwoFactorService(
         ApplicationDbContext context,
            EmailService emailService,
       EncryptionService encryptionService,
            ILogger<TwoFactorService> logger)
   {
            _context = context;
       _emailService = emailService;
 _encryptionService = encryptionService;
            _logger = logger;
        }

 // Generate 6-digit OTP
        public string GenerateOtp()
        {
using (var rng = RandomNumberGenerator.Create())
            {
byte[] randomBytes = new byte[4];
   rng.GetBytes(randomBytes);
    int randomNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
     return (randomNumber % 900000 + 100000).ToString(); // 6-digit number
            }
        }

      // Send OTP via email
        public async Task<bool> SendOtpAsync(string email, string userName)
        {
 try
   {
       var member = await _context.Members
          .FirstOrDefaultAsync(m => m.Email == email);

                if (member == null)
     return false;

       // Generate OTP
          var otp = GenerateOtp();

     // Store OTP in database
     member.CurrentOtp = otp;
     member.OtpExpiry = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes);
                await _context.SaveChangesAsync();

              // Send email
var emailSent = await _emailService.SendOtpEmailAsync(email, otp, userName);

          _logger.LogInformation($"OTP sent to {email}: {otp} (expires in {OtpExpiryMinutes} minutes)");

          return emailSent;
          }
    catch (Exception ex)
      {
           _logger.LogError(ex, $"Error sending OTP to {email}");
      return false;
      }
        }

 // Validate OTP
     public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
   try
            {
    var member = await _context.Members
   .FirstOrDefaultAsync(m => m.Email == email);

                if (member == null)
             return false;

  // Check if OTP exists and is not expired
  if (string.IsNullOrEmpty(member.CurrentOtp) || 
   member.OtpExpiry == null || 
           member.OtpExpiry < DateTime.UtcNow)
             {
   _logger.LogWarning($"OTP expired or missing for {email}");
        return false;
    }

        // Validate OTP
       bool isValid = member.CurrentOtp == otp;

       if (isValid)
                {
         // Clear OTP after successful validation
   member.CurrentOtp = null;
           member.OtpExpiry = null;
          await _context.SaveChangesAsync();
        _logger.LogInformation($"OTP validated successfully for {email}");
         }
    else
    {
   _logger.LogWarning($"Invalid OTP attempt for {email}");
      }

                return isValid;
        }
     catch (Exception ex)
            {
   _logger.LogError(ex, $"Error validating OTP for {email}");
                return false;
            }
      }

        // Enable 2FA for a member
      public async Task<bool> EnableTwoFactorAsync(int memberId)
        {
  try
     {
          var member = await _context.Members.FindAsync(memberId);
  if (member == null)
             return false;

           member.IsTwoFactorEnabled = true;
     await _context.SaveChangesAsync();

          return true;
         }
        catch (Exception ex)
  {
      _logger.LogError(ex, $"Error enabling 2FA for member {memberId}");
           return false;
   }
  }

 // Disable 2FA for a member
     public async Task<bool> DisableTwoFactorAsync(int memberId)
        {
            try
    {
       var member = await _context.Members.FindAsync(memberId);
                if (member == null)
 return false;

        member.IsTwoFactorEnabled = false;
          member.CurrentOtp = null;
        member.OtpExpiry = null;
       await _context.SaveChangesAsync();

            return true;
         }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error disabling 2FA for member {memberId}");
 return false;
     }
      }

    // Generate backup codes (10 codes, 8 digits each)
        public List<string> GenerateBackupCodes(int count = 10)
        {
            var codes = new List<string>();
          using (var rng = RandomNumberGenerator.Create())
 {
              for (int i = 0; i < count; i++)
           {
  byte[] randomBytes = new byte[4];
    rng.GetBytes(randomBytes);
              int randomNumber = Math.Abs(BitConverter.ToInt32(randomBytes, 0));
    var code = (randomNumber % 90000000 + 10000000).ToString(); // 8-digit number
      
  // Format as XXXX XXXX
          codes.Add($"{code.Substring(0, 4)} {code.Substring(4, 4)}");
           }
      }
 return codes;
 }

        // Store backup codes (encrypted)
        public async Task<List<string>> StoreBackupCodesAsync(int memberId)
        {
            try
            {
 var member = await _context.Members.FindAsync(memberId);
   if (member == null)
  return new List<string>();

    var codes = GenerateBackupCodes();
   var codesString = string.Join(",", codes);
         
     // Encrypt backup codes before storing
         member.BackupCodes = _encryptionService.Encrypt(codesString);
 await _context.SaveChangesAsync();

    _logger.LogInformation($"Generated {codes.Count} backup codes for member {memberId}");

       return codes;
            }
            catch (Exception ex)
   {
                _logger.LogError(ex, $"Error storing backup codes for member {memberId}");
      return new List<string>();
 }
        }

   // Validate and consume a backup code
        public async Task<bool> ValidateBackupCodeAsync(int memberId, string code)
 {
     try
{
          var member = await _context.Members.FindAsync(memberId);
         if (member == null || string.IsNullOrEmpty(member.BackupCodes))
          return false;

          // Decrypt backup codes
    var decryptedCodes = _encryptionService.Decrypt(member.BackupCodes);
 var codesList = decryptedCodes.Split(',').ToList();

            // Check if code exists
                if (codesList.Contains(code))
       {
   // Remove used code
  codesList.Remove(code);
             
         // Re-encrypt and save
        if (codesList.Any())
         {
     member.BackupCodes = _encryptionService.Encrypt(string.Join(",", codesList));
   }
    else
          {
           member.BackupCodes = null;
 }
   
          await _context.SaveChangesAsync();
       _logger.LogInformation($"Backup code used for member {memberId}. Remaining codes: {codesList.Count}");

              return true;
                }

       return false;
            }
      catch (Exception ex)
            {
        _logger.LogError(ex, $"Error validating backup code for member {memberId}");
      return false;
   }
    }
    }
}
