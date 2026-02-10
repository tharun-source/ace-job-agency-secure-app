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

   // Security Fix: Don't log token at all
   _logger.LogInformation("Validating CAPTCHA token");

    var response = await _httpClient.PostAsync(
       $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
  null);

   var jsonString = await response.Content.ReadAsStringAsync();
   _logger.LogInformation("CAPTCHA response received");

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
    _logger.LogWarning("CAPTCHA validation failed. Error codes: {ErrorCodes}", string.Join(", ", captchaResponse.ErrorCodes ?? Array.Empty<string>()));
     return false;
    }

    if (captchaResponse.Score < 0.5)
      {
   _logger.LogWarning("CAPTCHA score too low: {Score}", captchaResponse.Score);
    return false;
 }

          _logger.LogInformation("? CAPTCHA validation successful! Score: {Score}, Action: {Action}", captchaResponse.Score, captchaResponse.Action);
        return true;
   }
         catch (Exception ex)
        {
   _logger.LogError(ex, "Error validating CAPTCHA");
        return false;
  }
        }

        private class CaptchaResponse
     {
        [JsonPropertyName("success")]
            public bool Success { get; set; }

[JsonPropertyName("challenge_ts")]
        public string ChallengeTs { get; set; } = string.Empty;

      [JsonPropertyName("hostname")]
public string Hostname { get; set; } = string.Empty;

       [JsonPropertyName("error-codes")]
            public string[] ErrorCodes { get; set; } = Array.Empty<string>();

     [JsonPropertyName("score")]
 public double Score { get; set; }

            [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty;
        }
    }
}
