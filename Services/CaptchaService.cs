using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application_Security_Asgnt_wk12.Services
{
    public class CaptchaService
   {
private readonly IConfiguration _configuration;
 private readonly ILogger<CaptchaService> _logger;
      private readonly HttpClient _httpClient;

  public CaptchaService(IConfiguration configuration, ILogger<CaptchaService> logger, HttpClient httpClient)
{
    _configuration = configuration;
     _logger = logger;
     _httpClient = httpClient;
 }

   public async Task<bool> ValidateCaptchaAsync(string token)
 {
       try
    {
       // TEMPORARY: Bypass reCAPTCHA validation for testing
    // TODO: Remove this after getting proper v3 keys
            _logger.LogWarning("?? CAPTCHA validation BYPASSED for testing!");
      return true;

            /* ORIGINAL CODE - Enable after getting new keys
     if (string.IsNullOrEmpty(token))
    {
_logger.LogWarning("CAPTCHA token is empty");
return false;
   }

     var secretKey = _configuration["ReCaptcha:SecretKey"];
   if (string.IsNullOrEmpty(secretKey))
     {
    _logger.LogWarning("ReCaptcha secret key not configured");
  return false;
      }

       _logger.LogInformation($"Validating CAPTCHA with token: {token.Substring(0, Math.Min(20, token.Length))}...");

 var response = await _httpClient.PostAsync(
        $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
     null);

   var jsonString = await response.Content.ReadAsStringAsync();
         _logger.LogInformation($"CAPTCHA response: {jsonString}");

        var options = new JsonSerializerOptions
     {
     PropertyNameCaseInsensitive = true
     };
 var captchaResponse = JsonSerializer.Deserialize<CaptchaResponse>(jsonString, options);

   if (captchaResponse == null)
    {
   _logger.LogWarning("Failed to deserialize CAPTCHA response");
     return false;
}

   if (!captchaResponse.Success)
   {
           _logger.LogWarning($"CAPTCHA validation failed. Error codes: {string.Join(", ", captchaResponse.ErrorCodes ?? Array.Empty<string>())}");
 return false;
      }

  if (captchaResponse.Score < 0.1)
   {
    _logger.LogWarning($"CAPTCHA score too low: {captchaResponse.Score}");
      return false;
     }

  _logger.LogInformation($"? CAPTCHA validation successful! Score: {captchaResponse.Score}, Action: {captchaResponse.Action}");
  return true;
  */
   }
   catch (Exception ex)
    {
    _logger.LogError(ex, "Error validating CAPTCHA");
 return true; // Return true for testing
   }
        }

 private class CaptchaResponse
  {
      [JsonPropertyName("success")]
   public bool Success { get; set; }

 [JsonPropertyName("challenge_ts")]
    public string ChallengeTs { get; set; }

 [JsonPropertyName("hostname")]
   public string Hostname { get; set; }

    [JsonPropertyName("error-codes")]
   public string[] ErrorCodes { get; set; }

 [JsonPropertyName("score")]
        public double Score { get; set; }

  [JsonPropertyName("action")]
 public string Action { get; set; }
 }
    }
}
