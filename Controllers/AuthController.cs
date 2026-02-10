using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models;
using Application_Security_Asgnt_wk12.Models.DTOs;
using Application_Security_Asgnt_wk12.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Application_Security_Asgnt_wk12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly EncryptionService _encryptionService;
        private readonly SessionService _sessionService;
        private readonly AuditService _auditService;
        private readonly FileUploadService _fileUploadService;
        private readonly CaptchaService _captchaService;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;
        private readonly TwoFactorService _twoFactorService;

        public AuthController(
       ApplicationDbContext context,
       PasswordService passwordService,
            EncryptionService encryptionService,
  SessionService sessionService,
  AuditService auditService,
          FileUploadService fileUploadService,
  CaptchaService captchaService,
   EmailService emailService,
      ILogger<AuthController> logger,
    TwoFactorService twoFactorService)
        {
 _context = context;
 _passwordService = passwordService;
        _encryptionService = encryptionService;
            _sessionService = sessionService;
 _auditService = auditService;
 _fileUploadService = fileUploadService;
   _captchaService = captchaService;
            _emailService = emailService;
      _logger = logger;
      _twoFactorService = twoFactorService;
  }

   // Test endpoint to verify everything is working
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
  try
            {
var canConnect = await _context.Database.CanConnectAsync();
          var memberCount = await _context.Members.CountAsync();
  
      return Ok(new
          {
        message = "System is operational",
           databaseConnected = canConnect,
       memberCount = memberCount,
 encryptionTest = _encryptionService.Encrypt("test"),
        passwordTest = _passwordService.HashPassword("test") != null
       });
    }
            catch (Exception ex)
            {
        return StatusCode(500, new
       {
 message = "System test failed",
         error = ex.Message,
 innerError = ex.InnerException?.Message
     });
      }
   }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
       var userAgent = Request.Headers["User-Agent"].ToString();

       try
    {
      _logger.LogInformation("=== Registration attempt started ===");
  // Security Fix: Don't log user input (email, names)
  _logger.LogInformation("New registration attempt received");

    // Validate CAPTCHA
     var isCaptchaValid = await _captchaService.ValidateCaptchaAsync(dto.CaptchaToken);
      if (!isCaptchaValid)
  {
        await _auditService.LogActionAsync("0", "REGISTER_FAILED_CAPTCHA", ipAddress, userAgent);
return BadRequest(new { message = "Invalid CAPTCHA. Please try again." });
           }

        // Validate model state
        if (!ModelState.IsValid)
     {
      _logger.LogWarning("ModelState is invalid:");
   foreach (var error in ModelState)
   {
       _logger.LogWarning($"  {error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
        }
     var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
   return BadRequest(new { message = "Validation failed", errors = errors });
  }

 // Sanitize inputs
      dto.FirstName = SanitizeInput(dto.FirstName);
      dto.LastName = SanitizeInput(dto.LastName);
   dto.Email = SanitizeInput(dto.Email.ToLower());

      // Check if email already exists
  var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == dto.Email);
    if (existingMember != null)
   {
 // Security Fix: Don't log email address
 await _auditService.LogActionAsync("0", "REGISTER_FAILED_DUPLICATE_EMAIL", ipAddress, userAgent);
   return BadRequest(new { message = "Email address is already registered." });
      }

      // Validate password strength
 if (!_passwordService.ValidatePasswordStrength(dto.Password, out string passwordError))
    {
  return BadRequest(new { message = passwordError });
        }

     // Validate age (must be at least 18)
      var age = DateTime.Today.Year - dto.DateOfBirth.Year;
       if (dto.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
    if (age < 18)
     {
             return BadRequest(new { message = "You must be at least 18 years old to register." });
 }

       // Upload resume
 var resumeUpload = await _fileUploadService.UploadResumeAsync(dto.Resume, 0);
      if (!resumeUpload.Success)
      {
         return BadRequest(new { message = resumeUpload.ErrorMessage });
          }

    // Upload photo (optional)
      string photoPath = "";  // Default to empty string instead of null
        if (dto.Photo != null)
     {
   var photoUpload = await _fileUploadService.UploadPhotoAsync(dto.Photo, 0);
      if (!photoUpload.Success)
             {
      _fileUploadService.DeleteFile(resumeUpload.FilePath);
    return BadRequest(new { message = photoUpload.ErrorMessage });
     }
     photoPath = photoUpload.FilePath;
    }

  // Create member
        var member = new Member
    {
      FirstName = dto.FirstName,
         LastName = dto.LastName,
     Gender = dto.Gender,
          NRICEncrypted = _encryptionService.Encrypt(dto.NRIC),
Email = dto.Email,
       PasswordHash = _passwordService.HashPassword(dto.Password),
     DateOfBirth = dto.DateOfBirth,
          ResumePath = resumeUpload.FilePath,
    WhoAmI = SanitizeHtmlInput(dto.WhoAmI) ?? "",
 PhotoPath = photoPath,
  CreatedDate = DateTime.UtcNow,
           LastPasswordChangedDate = DateTime.UtcNow,
  PasswordHistory = string.Empty,
             FailedLoginAttempts = 0
  };

        _context.Members.Add(member);
           await _context.SaveChangesAsync();

    // Update file names with actual member ID
if (resumeUpload.FilePath != null)
         {
        var newResumeUpload = await _fileUploadService.UploadResumeAsync(dto.Resume, member.Id);
      _fileUploadService.DeleteFile(resumeUpload.FilePath);
    member.ResumePath = newResumeUpload.FilePath;
     }

   if (!string.IsNullOrEmpty(photoPath) && dto.Photo != null)
    {
            var newPhotoUpload = await _fileUploadService.UploadPhotoAsync(dto.Photo, member.Id);
   _fileUploadService.DeleteFile(photoPath);
  member.PhotoPath = newPhotoUpload.FilePath;
   }

    await _context.SaveChangesAsync();

// Log audit
      await _auditService.LogActionAsync(member.Id.ToString(), "REGISTER_SUCCESS", ipAddress, userAgent);

  // Security Fix: Don't log email address
    _logger.LogInformation("Registration successful for user ID: {MemberId}", member.Id);
    return Ok(new { message = "Registration successful! You can now login." });
   }
       catch (Exception ex)
          {
      _logger.LogError(ex, "Error during registration");
        _logger.LogError("Exception details - Type: {Type}, Message: {Message}, StackTrace: {StackTrace}", 
 ex.GetType().Name, ex.Message, ex.StackTrace);
            
  if (ex.InnerException != null)
     {
    _logger.LogError("Inner exception - Type: {Type}, Message: {Message}", 
     ex.InnerException.GetType().Name, ex.InnerException.Message);
    }
            
    await _auditService.LogActionAsync("0", "REGISTER_ERROR", ipAddress, userAgent, ex.Message);
     return StatusCode(500, new { message = "An error occurred during registration.", details = ex.Message, innerDetails = ex.InnerException?.Message });
   }
 }

        [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
  var userAgent = Request.Headers["User-Agent"].ToString();

  try
    {
    // Validate CAPTCHA
    var isCaptchaValid = await _captchaService.ValidateCaptchaAsync(dto.CaptchaToken);
      if (!isCaptchaValid)
     {
 await _auditService.LogActionAsync("0", "LOGIN_FAILED_CAPTCHA", ipAddress, userAgent);
          return BadRequest(new { message = "Invalid CAPTCHA. Please try again." });
     }

 // Sanitize email
      var email = SanitizeInput(dto.Email.ToLower());

  // Find member
       var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
 if (member == null)
   {
     // Security Fix: Don't log email in failed login attempts
        await _auditService.LogActionAsync("0", "LOGIN_FAILED_INVALID_EMAIL", ipAddress, userAgent);
        return Unauthorized(new { message = "Invalid email or password." });
    }

// Check if account is locked
    if (member.LockedOutUntil.HasValue && member.LockedOutUntil.Value > DateTime.UtcNow)
       {
var remainingMinutes = (int)(member.LockedOutUntil.Value - DateTime.UtcNow).TotalMinutes;
      await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_FAILED_ACCOUNT_LOCKED", ipAddress, userAgent);
           return Unauthorized(new { message = $"Account is locked. Please try again after {remainingMinutes} minutes." });
        }

      // Verify password
   if (!_passwordService.VerifyPassword(dto.Password, member.PasswordHash))
    {
  member.FailedLoginAttempts++;
 
 // Lock account after 3 failed attempts
     if (member.FailedLoginAttempts >= 3)
     {
    member.LockedOutUntil = DateTime.UtcNow.AddMinutes(15);
   await _context.SaveChangesAsync();
         await _auditService.LogActionAsync(member.Id.ToString(), "ACCOUNT_LOCKED", ipAddress, userAgent);
       return Unauthorized(new { message = "Account locked due to multiple failed login attempts. Please try again after 15 minutes." });
    }

         await _context.SaveChangesAsync();
       await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_FAILED_INVALID_PASSWORD", ipAddress, userAgent);
     return Unauthorized(new { message = "Invalid email or password." });
}

   // Check password age (must change password after 90 days)
        if (member.LastPasswordChangedDate.HasValue)
{
       // Check if password has expired
var passwordAgeStatus = _passwordService.GetPasswordAgeStatus(member.LastPasswordChangedDate);

     if (passwordAgeStatus.IsExpired)
  {
     await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_BLOCKED_PASSWORD_EXPIRED", ipAddress, userAgent);
 return Unauthorized(new { 
     message = "Your password has expired. Please reset your password using 'Forgot Password'.", 
     requirePasswordReset = true,
        daysExpired = passwordAgeStatus.DaysSinceLastChange - PasswordService.MaximumPasswordAgeDays
        });
   }
     }

   // Check if 2FA is enabled
    if (member.IsTwoFactorEnabled)
  {
   // Generate and send OTP
   var otpSent = await _twoFactorService.SendOtpAsync(member.Email, $"{member.FirstName} {member.LastName}");
     
  if (!otpSent)
    {
     await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_2FA_OTP_SEND_FAILED", ipAddress, userAgent);
        return StatusCode(500, new { message = "Failed to send OTP. Please try again." });
 }
        
  // Reset failed login attempts (password was correct)
       member.FailedLoginAttempts = 0;
       member.LockedOutUntil = null;
     await _context.SaveChangesAsync();
    
       await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_2FA_OTP_SENT", ipAddress, userAgent);
       
    return Ok(new
  {
   message = "OTP sent to your email. Please verify to complete login.",
 requireOtp = true,
     email = member.Email
        });
  }

     // Reset failed login attempts
        member.FailedLoginAttempts = 0;
      member.LockedOutUntil = null;
      member.LastLoginDate = DateTime.UtcNow;
     await _context.SaveChangesAsync();

   // Security Enhancement: Invalidate all other sessions for this user FIRST
      // This prevents multiple simultaneous logins from different locations
        // The session service uses semaphore locking to prevent race conditions
        _logger.LogInformation("Invalidating all existing sessions for user ID: {MemberId}", member.Id);
        await _sessionService.InvalidateAllUserSessionsAsync(member.Id);
          await _auditService.LogActionAsync(member.Id.ToString(), "ALL_SESSIONS_INVALIDATED", ipAddress, userAgent);

    // Create NEW session AFTER invalidation (only one should be active)
    _logger.LogInformation("Creating new session for user ID: {MemberId}", member.Id);
var session = await _sessionService.CreateSessionAsync(member.Id, ipAddress, userAgent);
        _logger.LogInformation("New session created with ID: {SessionId} for user ID: {MemberId}", session.SessionId, member.Id);
       
  // Store session in HTTP session
 HttpContext.Session.SetString("SessionId", session.SessionId);
       HttpContext.Session.SetInt32("MemberId", member.Id);
  HttpContext.Session.SetString("Email", member.Email);

         await _auditService.LogActionAsync(member.Id.ToString(), "LOGIN_SUCCESS", ipAddress, userAgent);

       return Ok(new 
   { 
     message = "Login successful!",
sessionId = session.SessionId,
    member = new
         {
   id = member.Id,
  firstName = member.FirstName,
    lastName = member.LastName,
  email = member.Email
   }
     });
     }
   catch (Exception ex)
      {
   _logger.LogError(ex, "Error during login");
     await _auditService.LogActionAsync("0", "LOGIN_ERROR", ipAddress, userAgent, ex.Message);
          return StatusCode(500, new { message = "An error occurred during login." });
     }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
      {
   var sessionId = HttpContext.Session.GetString("SessionId");
          var memberId = HttpContext.Session.GetInt32("MemberId");
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
         var userAgent = Request.Headers["User-Agent"].ToString();

  if (sessionId != null)
       {
       await _sessionService.InvalidateSessionAsync(sessionId);
     await _auditService.LogActionAsync(memberId?.ToString() ?? "0", "LOGOUT", ipAddress, userAgent);
      }

      HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful!" });
  }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
   {
    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
 var userAgent = Request.Headers["User-Agent"].ToString();

  try
  {
  // Sanitize email
    var email = SanitizeInput(dto.Email.ToLower());

     // Find member
     var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
    
         // For security, always return success even if email doesn't exist
 // This prevents email enumeration attacks
 if (member == null)
       {
  // Security Fix: Don't log email address
       await _auditService.LogActionAsync("0", "FORGOT_PASSWORD_INVALID_EMAIL", ipAddress, userAgent, "[redacted]");
        await Task.Delay(1000); // Simulate processing time
   return Ok(new { message = "If that email exists, a password reset link has been sent." });
         }

  // Generate reset token
       var resetToken = _passwordService.GeneratePasswordResetToken();
     var tokenExpiry = _passwordService.GetResetTokenExpiry();

// Save token to database
     member.PasswordResetToken = resetToken;
      member.PasswordResetTokenExpiry = tokenExpiry;
        await _context.SaveChangesAsync();

              // Generate reset link
      var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password.html?token={resetToken}&email={Uri.EscapeDataString(email)}";

     // Send email
         var emailSent = await _emailService.SendPasswordResetEmailAsync(
        email,
         resetLink,
    $"{member.FirstName} {member.LastName}"
        );

     if (emailSent)
{
        await _auditService.LogActionAsync(member.Id.ToString(), "FORGOT_PASSWORD_SUCCESS", ipAddress, userAgent);
     }
   else
       {
   await _auditService.LogActionAsync(member.Id.ToString(), "FORGOT_PASSWORD_EMAIL_FAILED", ipAddress, userAgent);
 }

     return Ok(new { message = "If that email exists, a password reset link has been sent." });
    }
   catch (Exception ex)
            {
     _logger.LogError(ex, "Error during forgot password");
    await _auditService.LogActionAsync("0", "FORGOT_PASSWORD_ERROR", ipAddress, userAgent, ex.Message);
 return StatusCode(500, new { message = "An error occurred. Please try again later." });
      }
     }

 [HttpPost("verify-otp")]
  public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
     {
         var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
  var userAgent = Request.Headers["User-Agent"].ToString();

  try
       {
   // Sanitize email
 var email = SanitizeInput(dto.Email.ToLower());

      // Find member
  var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
    if (member == null)
     {
    // Security Fix: Don't log email in failed OTP attempts
  await _auditService.LogActionAsync("0", "VERIFY_OTP_INVALID_EMAIL", ipAddress, userAgent);
 return Unauthorized(new { message = "Invalid verification code." });
    }

    // Validate OTP
  var isValid = await _twoFactorService.ValidateOtpAsync(email, dto.Otp);
 if (!isValid)
        {
    await _auditService.LogActionAsync(member.Id.ToString(), "VERIFY_OTP_FAILED", ipAddress, userAgent);
  return Unauthorized(new { message = "Invalid or expired verification code." });
       }

     // Update last login
             member.LastLoginDate = DateTime.UtcNow;
         await _context.SaveChangesAsync();

    // Create session
    var session = await _sessionService.CreateSessionAsync(member.Id, ipAddress, userAgent);
       
  // Store session in HTTP session
 HttpContext.Session.SetString("SessionId", session.SessionId);
       HttpContext.Session.SetInt32("MemberId", member.Id);
  HttpContext.Session.SetString("Email", member.Email);

         await _auditService.LogActionAsync(member.Id.ToString(), "VERIFY_OTP_SUCCESS_LOGIN_COMPLETE", ipAddress, userAgent);

     return Ok(new 
   { 
     message = "Login successful!",
        sessionId = session.SessionId,
    member = new
         {
      id = member.Id,
    firstName = member.FirstName,
      lastName = member.LastName,
  email = member.Email
               }
     });
            }
 catch (Exception ex)
   {
     _logger.LogError(ex, "Error during OTP verification");
           await _auditService.LogActionAsync("0", "VERIFY_OTP_ERROR", ipAddress, userAgent, ex.Message);
 return StatusCode(500, new { message = "An error occurred during verification." });
   }
  }

     [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
   {
       var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var userAgent = Request.Headers["User-Agent"].ToString();

     try
{
    // Sanitize inputs
         var email = SanitizeInput(dto.Email.ToLower());
    var token = dto.Token?.Trim();

  if (string.IsNullOrEmpty(token))
   {
      return BadRequest(new { message = "Invalid reset token." });
      }

   // Find member
    var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);
     if (member == null)
       {
       // Security Fix: Don't log email in failed reset attempts
      await _auditService.LogActionAsync("0", "RESET_PASSWORD_INVALID_EMAIL", ipAddress, userAgent);
return BadRequest(new { message = "Invalid reset token or email." });
  }

    // Validate token
                if (member.PasswordResetToken != token)
{
      await _auditService.LogActionAsync(member.Id.ToString(), "RESET_PASSWORD_INVALID_TOKEN", ipAddress, userAgent);
     return BadRequest(new { message = "Invalid reset token." });
       }

          // Check token expiry
     if (!_passwordService.IsResetTokenValid(member.PasswordResetTokenExpiry))
     {
        await _auditService.LogActionAsync(member.Id.ToString(), "RESET_PASSWORD_TOKEN_EXPIRED", ipAddress, userAgent);
   return BadRequest(new { message = "Reset token has expired. Please request a new one." });
     }

 // Validate password strength
 if (!_passwordService.ValidatePasswordStrength(dto.NewPassword, out string passwordError))
        {
   return BadRequest(new { message = passwordError });
    }

     // Hash new password
    var newPasswordHash = _passwordService.HashPassword(dto.NewPassword);

    // Check password history
   if (!_passwordService.CheckPasswordHistory(newPasswordHash, member.PasswordHistory))
   {
   return BadRequest(new { message = "You cannot reuse any of your last 2 passwords." });
    }

   // Update password and history
     member.PasswordHistory = _passwordService.AddToPasswordHistory(member.PasswordHash, member.PasswordHistory);
member.PasswordHash = newPasswordHash;
         member.LastPasswordChangedDate = DateTime.UtcNow;
       
         // Clear reset token
          member.PasswordResetToken = null;
        member.PasswordResetTokenExpiry = null;
    
          // Reset failed login attempts
 member.FailedLoginAttempts = 0;
       member.LockedOutUntil = null;

      await _context.SaveChangesAsync();
       await _auditService.LogActionAsync(member.Id.ToString(), "RESET_PASSWORD_SUCCESS", ipAddress, userAgent);

        return Ok(new { message = "Password has been reset successfully. You can now login with your new password." });
    }
        catch (Exception ex)
            {
     _logger.LogError(ex, "Error during password reset");
    await _auditService.LogActionAsync("0", "RESET_PASSWORD_ERROR", ipAddress, userAgent, ex.Message);
    return StatusCode(500, new { message = "An error occurred. Please try again later." });
            }
        }

     private string SanitizeInput(string? input)
        {
      if (string.IsNullOrEmpty(input))
   return string.Empty;

      // Remove any potential SQL injection or XSS characters
  input = input.Trim();
    input = Regex.Replace(input, @"[<>""']", "");
        return input;
        }

     private string? SanitizeHtmlInput(string? input)
      {
  if (string.IsNullOrEmpty(input))
           return null;

 // For "Who Am I" field, we allow special characters but encode them
          return System.Net.WebUtility.HtmlEncode(input.Trim());
      }
    }
}
