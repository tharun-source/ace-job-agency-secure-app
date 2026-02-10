using Application_Security_Asgnt_wk12.Services;

namespace Application_Security_Asgnt_wk12.Middleware
{
    public class SessionValidationMiddleware
    {
private readonly RequestDelegate _next;
  private readonly ILogger<SessionValidationMiddleware> _logger;

        public SessionValidationMiddleware(RequestDelegate next, ILogger<SessionValidationMiddleware> logger)
 {
            _next = next;
       _logger = logger;
        }

 public async Task InvokeAsync(HttpContext context, SessionService sessionService)
        {
            // Skip validation for certain paths
         var path = context.Request.Path.Value?.ToLower() ?? "";
   
          // List of paths that don't require session validation
         var publicPaths = new[] { 
                "/swagger", "/_framework", "/api/auth/",
      "/login.html", "/register.html", "/index.html", 
  "/forgot-password.html", "/reset-password.html",
           "/404.html", "/test-captcha.html",
      "/verify-otp.html", "/css/", "/js/", "/images/"
          };

     if (publicPaths.Any(p => path.Contains(p)))
            {
  await _next(context);
   return;
    }

        // Check if session exists for protected resources
       var sessionId = context.Session.GetString("SessionId");
  
            if (!string.IsNullOrEmpty(sessionId))
        {
          var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
  var userAgent = context.Request.Headers["User-Agent"].ToString();

    // Validate session - checks if still active in database
        var isValid = await sessionService.ValidateSessionAsync(sessionId, ipAddress, userAgent);
       
    if (!isValid)
     {
       // Session was invalidated (e.g., user logged in elsewhere)
    _logger.LogWarning("Session {SessionId} invalidated for path {Path}", sessionId, path);
    context.Session.Clear();

    // Redirect HTML requests to login, return 401 for API calls
        if (path.EndsWith(".html") || !path.Contains("/api/"))
             {
      context.Response.Redirect("/login.html");
       return;
          }
              else
    {
        context.Response.StatusCode = 401;
       await context.Response.WriteAsJsonAsync(new { message = "Session invalid or expired. Please login again." });
    return;
         }
       }

    // Extend session on activity
         await sessionService.ExtendSessionAsync(sessionId);
       }
         else if (path.EndsWith(".html") && !path.Contains("/login.html") && !path.Contains("/index.html"))
            {
      // No session for protected HTML pages - redirect to login
   _logger.LogWarning("No session found for protected path {Path}", path);
                context.Response.Redirect("/login.html");
   return;
            }

     await _next(context);
        }
    }
}
