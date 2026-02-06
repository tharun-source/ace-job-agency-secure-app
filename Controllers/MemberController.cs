using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models.DTOs;
using Application_Security_Asgnt_wk12.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Application_Security_Asgnt_wk12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
     private readonly ApplicationDbContext _context;
  private readonly PasswordService _passwordService;
        private readonly EncryptionService _encryptionService;
     private readonly AuditService _auditService;
        private readonly ILogger<MemberController> _logger;
   private readonly IWebHostEnvironment _environment;

        public MemberController(
ApplicationDbContext context,
      PasswordService passwordService,
       EncryptionService encryptionService,
  AuditService auditService,
      ILogger<MemberController> logger,
 IWebHostEnvironment environment)
        {
          _context = context;
      _passwordService = passwordService;
        _encryptionService = encryptionService;
  _auditService = auditService;
 _logger = logger;
     _environment = environment;
     }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
     var memberId = HttpContext.Session.GetInt32("MemberId");
      if (memberId == null)
   {
            return Unauthorized(new { message = "Please login to view profile." });
 }

         var member = await _context.Members.FindAsync(memberId.Value);
            if (member == null)
   {
            return NotFound(new { message = "Member not found." });
      }

       var profile = new MemberProfileDto
        {
       Id = member.Id,
       FirstName = member.FirstName,
   LastName = member.LastName,
      Gender = member.Gender,
 NRICDecrypted = _encryptionService.Decrypt(member.NRICEncrypted),
 Email = member.Email,
    DateOfBirth = member.DateOfBirth,
       ResumePath = member.ResumePath,
        WhoAmI = member.WhoAmI,
       PhotoPath = member.PhotoPath,
 CreatedDate = member.CreatedDate,
    LastLoginDate = member.LastLoginDate
 };

  return Ok(profile);
        }

        [HttpPost("change-password")]
     public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
          var memberId = HttpContext.Session.GetInt32("MemberId");
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = Request.Headers["User-Agent"].ToString();

            if (memberId == null)
       {
      return Unauthorized(new { message = "Please login to change password." });
   }

   try
     {
             var member = await _context.Members.FindAsync(memberId.Value);
   if (member == null)
             {
      return NotFound(new { message = "Member not found." });
        }

// Verify current password
    if (!_passwordService.VerifyPassword(dto.CurrentPassword, member.PasswordHash))
        {
         await _auditService.LogActionAsync(member.Id.ToString(), "CHANGE_PASSWORD_FAILED", ipAddress, userAgent);
    return BadRequest(new { message = "Current password is incorrect." });
 }

 // Check if new password is same as current
     if (_passwordService.VerifyPassword(dto.NewPassword, member.PasswordHash))
         {
           return BadRequest(new { message = "New password must be different from current password." });
    }

        // Validate new password strength
       if (!_passwordService.ValidatePasswordStrength(dto.NewPassword, out string passwordError))
   {
   return BadRequest(new { message = passwordError });
      }

           // Check if passwords match
     if (dto.NewPassword != dto.ConfirmNewPassword)
           {
 return BadRequest(new { message = "Passwords do not match." });
    }

       // Check minimum password age (cannot change within 5 minutes)
  if (member.LastPasswordChangedDate.HasValue)
     {
           var minutesSinceLastChange = (DateTime.UtcNow - member.LastPasswordChangedDate.Value).TotalMinutes;
if (minutesSinceLastChange < 5)
 {
 return BadRequest(new { message = "You must wait at least 5 minutes before changing your password again." });
     }
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

    await _context.SaveChangesAsync();
     await _auditService.LogActionAsync(member.Id.ToString(), "CHANGE_PASSWORD_SUCCESS", ipAddress, userAgent);

  return Ok(new { message = "Password changed successfully!" });
       }
    catch (Exception ex)
   {
    _logger.LogError(ex, "Error changing password for member {MemberId}", memberId);
     await _auditService.LogActionAsync(memberId.ToString()!, "CHANGE_PASSWORD_ERROR", ipAddress, userAgent, ex.Message);
                return StatusCode(500, new { message = "An error occurred while changing password." });
    }
 }

        [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs()
        {
     var memberId = HttpContext.Session.GetInt32("MemberId");
     if (memberId == null)
        {
   return Unauthorized(new { message = "Please login to view audit logs." });
   }

            var logs = await _auditService.GetMemberAuditLogsAsync(memberId.Value.ToString());
  return Ok(logs);
      }

        [HttpGet("download-resume")]
  public async Task<IActionResult> DownloadResume()
      {
    var memberId = HttpContext.Session.GetInt32("MemberId");
   var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var userAgent = Request.Headers["User-Agent"].ToString();

         if (memberId == null)
 {
       return Unauthorized(new { message = "Please login to download resume." });
       }

 try
          {
       var member = await _context.Members.FindAsync(memberId.Value);
              if (member == null)
        {
    return NotFound(new { message = "Member not found." });
            }

         if (string.IsNullOrEmpty(member.ResumePath))
      {
        return NotFound(new { message = "Resume not found." });
       }

           var filePath = Path.Combine(_environment.WebRootPath, member.ResumePath.TrimStart('/'));
         
    if (!System.IO.File.Exists(filePath))
  {
      return NotFound(new { message = "Resume file not found on server." });
    }

    var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
      var fileName = Path.GetFileName(filePath);
                var contentType = fileName.EndsWith(".pdf") ? "application/pdf" : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            await _auditService.LogActionAsync(member.Id.ToString(), "DOWNLOAD_RESUME", ipAddress, userAgent);

      return File(fileBytes, contentType, fileName);
            }
          catch (Exception ex)
            {
         _logger.LogError(ex, "Error downloading resume for member {MemberId}", memberId);
       return StatusCode(500, new { message = "An error occurred while downloading resume." });
   }
        }

        [HttpGet("download-photo")]
     public async Task<IActionResult> DownloadPhoto()
        {
     var memberId = HttpContext.Session.GetInt32("MemberId");
   var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
      var userAgent = Request.Headers["User-Agent"].ToString();

 if (memberId == null)
    {
        return Unauthorized(new { message = "Please login to download photo." });
            }

     try
       {
      var member = await _context.Members.FindAsync(memberId.Value);
  if (member == null)
    {
 return NotFound(new { message = "Member not found." });
   }

      if (string.IsNullOrEmpty(member.PhotoPath))
  {
             return NotFound(new { message = "Photo not found." });
  }

 var filePath = Path.Combine(_environment.WebRootPath, member.PhotoPath.TrimStart('/'));
        
            if (!System.IO.File.Exists(filePath))
 {
    return NotFound(new { message = "Photo file not found on server." });
             }

      var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
     var fileName = Path.GetFileName(filePath);
     var extension = Path.GetExtension(fileName).ToLower();
     var contentType = extension switch
      {
         ".jpg" or ".jpeg" => "image/jpeg",
       ".png" => "image/png",
 ".gif" => "image/gif",
  _ => "application/octet-stream"
    };

     await _auditService.LogActionAsync(member.Id.ToString(), "DOWNLOAD_PHOTO", ipAddress, userAgent);

        return File(fileBytes, contentType, fileName);
    }
            catch (Exception ex)
         {
_logger.LogError(ex, "Error downloading photo for member {MemberId}", memberId);
        return StatusCode(500, new { message = "An error occurred while downloading photo." });
    }
        }

        [HttpPost("update-profile")]
      public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
       var memberId = HttpContext.Session.GetInt32("MemberId");
 var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();

  if (memberId == null)
  {
                return Unauthorized(new { message = "Please login to update profile." });
 }

            try
            {
     var member = await _context.Members.FindAsync(memberId.Value);
     if (member == null)
      {
        return NotFound(new { message = "Member not found." });
     }

         // Validate age (must be at least 18)
       var age = DateTime.Today.Year - dto.DateOfBirth.Year;
             if (dto.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
            if (age < 18)
{
              return BadRequest(new { message = "You must be at least 18 years old." });
       }

                // Update basic information
         member.FirstName = dto.FirstName.Trim();
       member.LastName = dto.LastName.Trim();
       member.Gender = dto.Gender;
    member.DateOfBirth = dto.DateOfBirth;
    member.WhoAmI = string.IsNullOrWhiteSpace(dto.WhoAmI) 
         ? "" 
                 : System.Net.WebUtility.HtmlEncode(dto.WhoAmI.Trim());

       // Handle resume upload if provided
            if (dto.Resume != null)
 {
        var fileUploadService = HttpContext.RequestServices.GetRequiredService<FileUploadService>();
  var resumeUpload = await fileUploadService.UploadResumeAsync(dto.Resume, member.Id);
      
       if (!resumeUpload.Success)
              {
   return BadRequest(new { message = resumeUpload.ErrorMessage });
           }

           // Delete old resume if exists
        if (!string.IsNullOrEmpty(member.ResumePath))
            {
     fileUploadService.DeleteFile(member.ResumePath);
  }

             member.ResumePath = resumeUpload.FilePath;
       }

  // Handle photo upload if provided
     if (dto.Photo != null)
             {
       var fileUploadService = HttpContext.RequestServices.GetRequiredService<FileUploadService>();
        var photoUpload = await fileUploadService.UploadPhotoAsync(dto.Photo, member.Id);
      
     if (!photoUpload.Success)
        {
             return BadRequest(new { message = photoUpload.ErrorMessage });
  }

            // Delete old photo if exists
  if (!string.IsNullOrEmpty(member.PhotoPath))
           {
        fileUploadService.DeleteFile(member.PhotoPath);
           }

      member.PhotoPath = photoUpload.FilePath;
          }

      await _context.SaveChangesAsync();
           await _auditService.LogActionAsync(member.Id.ToString(), "UPDATE_PROFILE_SUCCESS", ipAddress, userAgent);

      return Ok(new { message = "Profile updated successfully!" });
  }
        catch (Exception ex)
      {
         _logger.LogError(ex, "Error updating profile for member {MemberId}", memberId);
  await _auditService.LogActionAsync(memberId.ToString()!, "UPDATE_PROFILE_ERROR", ipAddress, userAgent, ex.Message);
   return StatusCode(500, new { message = "An error occurred while updating profile." });
 }
        }
    }
}
