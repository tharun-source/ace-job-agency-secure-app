namespace Application_Security_Asgnt_wk12.Services
{
    public class FileUploadService
  {
        private readonly string _uploadPath;
     private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;

     private static readonly string[] AllowedResumeExtensions = { ".pdf", ".docx" };
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
     private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
       _environment = environment;
     _logger = logger;
  _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
      
          // Ensure upload directories exist
 Directory.CreateDirectory(Path.Combine(_uploadPath, "resumes"));
  Directory.CreateDirectory(Path.Combine(_uploadPath, "photos"));
   }

   public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadResumeAsync(IFormFile file, int memberId)
        {
   try
    {
        // Validate file
     if (file == null || file.Length == 0)
          return (false, null, "No file uploaded");

      if (file.Length > MaxFileSize)
         return (false, null, "File size exceeds 5MB limit");

   var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
       if (!AllowedResumeExtensions.Contains(extension))
   return (false, null, "Only .pdf and .docx files are allowed for resumes");

      // Sanitize filename
      var fileName = $"{memberId}_{Guid.NewGuid()}{extension}";
 var filePath = Path.Combine(_uploadPath, "resumes", fileName);

    // Save file
   using (var stream = new FileStream(filePath, FileMode.Create))
   {
     await file.CopyToAsync(stream);
  }

        var relativePath = Path.Combine("uploads", "resumes", fileName).Replace("\\", "/");
  return (true, relativePath, null);
    }
    catch (Exception ex)
          {
       _logger.LogError(ex, "Error uploading resume for member {MemberId}", memberId);
    return (false, null, "An error occurred while uploading the file");
     }
  }

   public async Task<(bool Success, string? FilePath, string? ErrorMessage)> UploadPhotoAsync(IFormFile file, int memberId)
   {
     try
    {
    if (file == null || file.Length == 0)
    return (true, null, null); // Photo is optional

    if (file.Length > MaxFileSize)
     return (false, null, "File size exceeds 5MB limit");

       var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!AllowedImageExtensions.Contains(extension))
       return (false, null, "Only .jpg, .jpeg, .png, and .gif files are allowed for photos");

     // Sanitize filename
 var fileName = $"{memberId}_{Guid.NewGuid()}{extension}";
    var filePath = Path.Combine(_uploadPath, "photos", fileName);

     // Save file
    using (var stream = new FileStream(filePath, FileMode.Create))
{
            await file.CopyToAsync(stream);
          }

     var relativePath = Path.Combine("uploads", "photos", fileName).Replace("\\", "/");
        return (true, relativePath, null);
    }
    catch (Exception ex)
 {
   _logger.LogError(ex, "Error uploading photo for member {MemberId}", memberId);
     return (false, null, "An error occurred while uploading the file");
   }
        }

        public bool DeleteFile(string? filePath)
   {
     try
{
       if (string.IsNullOrEmpty(filePath))
       return true;

    var fullPath = Path.Combine(_environment.WebRootPath, filePath);
     if (File.Exists(fullPath))
{
   File.Delete(fullPath);
   }
           return true;
     }
 catch (Exception ex)
    {
      _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
       return false;
     }
        }
    }
}
