using Application_Security_Asgnt_wk12.Services;

namespace Application_Security_Asgnt_wk12.Middleware
{
    public class SessionValidationMiddleware
 {
 private readonly RequestDelegate _next;

     public SessionValidationMiddleware(RequestDelegate next)
 {
            _next = next;
 }

        public async Task InvokeAsync(HttpContext context, SessionService sessionService)
      {
   // Skip validation for certain paths
       var path = context.Request.Path.Value?.ToLower() ?? "";
   if (path.Contains("/auth/") || path.Contains("/swagger") || path.Contains("/_framework"))
   {
      await _next(context);
    return;
   }

  // Check if session exists
   if (context.Session.GetString("SessionId") != null)
   {
  var sessionId = context.Session.GetString("SessionId");
     var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
   var userAgent = context.Request.Headers["User-Agent"].ToString();

   // Validate session
      var isValid = await sessionService.ValidateSessionAsync(sessionId!, ipAddress, userAgent);
       
       if (!isValid)
    {
        // Clear invalid session
      context.Session.Clear();
       context.Response.StatusCode = 401;
 await context.Response.WriteAsJsonAsync(new { message = "Session invalid or expired" });
  return;
      }

   // Extend session on activity
    await sessionService.ExtendSessionAsync(sessionId!);
   }

      await _next(context);
}
    }
}
