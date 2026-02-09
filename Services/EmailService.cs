using System.Net;
using System.Net.Mail;

namespace Application_Security_Asgnt_wk12.Services
{
    public class EmailService
    {
      private readonly IConfiguration _configuration;
      private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
     {
       _configuration = configuration;
  _logger = logger;
    }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName)
        {
  try
{
       // For development/testing, just log the reset link
   if (_configuration.GetValue<bool>("Email:UseConsoleLog"))
  {
   // Security Fix: Redact sensitive information in logs
          var redactedEmail = RedactEmail(toEmail);
     _logger.LogInformation("Password reset requested for user: {RedactedEmail}", redactedEmail);
          Console.WriteLine($"\n========== PASSWORD RESET EMAIL ==========");
              Console.WriteLine($"To: {redactedEmail}");
     Console.WriteLine($"Subject: Password Reset Request");
           Console.WriteLine($"Reset Link: {resetLink}");
        Console.WriteLine($"Link expires in 15 minutes");
            Console.WriteLine($"==========================================\n");
   return true;
    }

     // Production email sending (configure SMTP settings in appsettings.json)
   var smtpHost = _configuration["Email:SmtpHost"];
        var smtpPort = _configuration.GetValue<int>("Email:SmtpPort");
    var smtpUser = _configuration["Email:SmtpUser"];
  var smtpPass = _configuration["Email:SmtpPassword"];
   var fromEmail = _configuration["Email:FromEmail"];
var fromName = _configuration["Email:FromName"];

          using var smtpClient = new SmtpClient(smtpHost, smtpPort)
         {
     Credentials = new NetworkCredential(smtpUser, smtpPass),
     EnableSsl = true
      };

   var mailMessage = new MailMessage
      {
  From = new MailAddress(fromEmail, fromName),
   Subject = "Password Reset Request - Ace Job Agency",
     Body = GetEmailBody(userName, resetLink),
      IsBodyHtml = true
       };

           mailMessage.To.Add(toEmail);

    await smtpClient.SendMailAsync(mailMessage);
    return true;
   }
      catch (Exception ex)
 {
          // Security Fix: Don't log email address in error messages
        _logger.LogError(ex, "Failed to send password reset email");
    return false;
            }
        }

  public async Task<bool> SendOtpEmailAsync(string toEmail, string otp, string userName)
        {
        try
        {
         // For development/testing, just log the OTP
    if (_configuration.GetValue<bool>("Email:UseConsoleLog"))
       {
    // Security Fix: Redact sensitive information in logs
      var redactedEmail = RedactEmail(toEmail);
   _logger.LogInformation("2FA OTP sent for user: {RedactedEmail}", redactedEmail);
    Console.WriteLine($"\n========== 2FA OTP CODE ==========");
    Console.WriteLine($"To: {redactedEmail}");
             Console.WriteLine($"User: {userName}");
          Console.WriteLine($"OTP Code: {otp}");
     Console.WriteLine($"Expires in: 5 minutes");
     Console.WriteLine($"===================================\n");
 return true;
   }

// Production email sending
     var smtpHost = _configuration["Email:SmtpHost"];
         var smtpPort = _configuration.GetValue<int>("Email:SmtpPort");
          var smtpUser = _configuration["Email:SmtpUser"];
           var smtpPass = _configuration["Email:SmtpPassword"];
       var fromEmail = _configuration["Email:FromEmail"];
       var fromName = _configuration["Email:FromName"];

         using var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
    Credentials = new NetworkCredential(smtpUser, smtpPass),
  EnableSsl = true
 };

      var mailMessage = new MailMessage
     {
  From = new MailAddress(fromEmail, fromName),
      Subject = "Your 2FA Verification Code - Ace Job Agency",
             Body = GetOtpEmailBody(userName, otp),
     IsBodyHtml = true
     };

      mailMessage.To.Add(toEmail);

      await smtpClient.SendMailAsync(mailMessage);
 return true;
            }
    catch (Exception ex)
            {
     // Security Fix: Don't log email address in error messages
   _logger.LogError(ex, "Failed to send OTP email");
return false;
  }
        }

   /// <summary>
        /// Redacts email address for security logging
        /// Example: john.doe@example.com becomes j***@e***.com
        /// </summary>
      private string RedactEmail(string email)
        {
          if (string.IsNullOrEmpty(email) || !email.Contains('@'))
    return "[redacted]";

      var parts = email.Split('@');
          var localPart = parts[0];
        var domainPart = parts[1];

    // Redact local part (keep first char)
            var redactedLocal = localPart.Length > 1 
                ? $"{localPart[0]}***" 
       : "***";

 // Redact domain (keep first char of domain name)
  var domainParts = domainPart.Split('.');
  var redactedDomain = domainParts.Length > 1
  ? $"{domainParts[0][0]}***.{domainParts[^1]}"
        : "***";

return $"{redactedLocal}@{redactedDomain}";
        }

   private string GetOtpEmailBody(string userName, string otp)
  {
      return $@"
       <!DOCTYPE html>
     <html>
   <head>
     <style>
         body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
   .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
   .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
     .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 5px 5px; }}
  .otp-code {{ 
     font-size: 32px; 
      font-weight: bold; 
  color: #667eea; 
       text-align: center; 
    padding: 20px; 
       background: white; 
        border-radius: 5px; 
      margin: 20px 0;
              letter-spacing: 5px;
  }}
     .warning {{ background: #fff3cd; border: 1px solid #ffeeba; color: #856404; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
 </style>
            </head>
            <body>
       <div class='container'>
        <div class='header'>
     <h1>?? Ace Job Agency</h1>
       <p>Two-Factor Authentication</p>
        </div>
  <div class='content'>
          <p>Hello {userName},</p>
      <p>Your verification code for login is:</p>
         <div class='otp-code'>{otp}</div>
            <p style='text-align: center; color: #666;'>Enter this code to complete your login</p>
      <div class='warning'>
    <strong>?? Security Notice:</strong>
             <ul style='margin: 10px 0;'>
      <li>This code will expire in <strong>5 minutes</strong></li>
  <li>Never share this code with anyone</li>
 <li>If you didn't request this code, please secure your account immediately</li>
         </ul>
          </div>
    </div>
         <div class='footer'>
      <p>© 2024 Ace Job Agency - Secure Member Management System</p>
  <p>This is an automated email. Please do not reply.</p>
   </div>
      </div>
  </body>
       </html>
            ";
        }

     private string GetEmailBody(string userName, string resetLink)
    {
  return $@"
 <!DOCTYPE html>
      <html>
  <head>
        <style>
            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
  .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
     .content {{ background: #f8f9fa; padding: 30px; border-radius: 0 0 5px 5px; }}
       .button {{ display: inline-block; padding: 15px 30px; background: #667eea; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
  .warning {{ background: #fff3cd; border: 1px solid #ffeeba; color: #856404; padding: 15px; border-radius: 5px; margin: 20px 0; }}
         </style>
    </head>
  <body>
      <div class='container'>
       <div class='header'>
<h1>?? Ace Job Agency</h1>
       <p>Password Reset Request</p>
             </div>
        <div class='content'>
        <p>Hello {userName},</p>
         <p>We received a request to reset your password. Click the button below to create a new password:</p>
     <div style='text-align: center;'>
         <a href='{resetLink}' class='button'>Reset Password</a>
     </div>
  <p>Or copy and paste this link into your browser:</p>
        <p style='word-break: break-all; background: white; padding: 10px; border-radius: 5px;'>{resetLink}</p>
  <div class='warning'>
               <strong>?? Important:</strong>
  <ul>
 <li>This link will expire in <strong>15 minutes</strong></li>
    <li>If you didn't request this reset, please ignore this email</li>
    <li>Your password will remain unchanged</li>
       </ul>
    </div>
             </div>
         <div class='footer'>
       <p>© 2024 Ace Job Agency - Secure Member Management System</p>
   <p>This is an automated email. Please do not reply.</p>
    </div>
      </div>
  </body>
        </html>
 ";
        }
    }
}
