using System.Net;
using System.Text.Json;

namespace Application_Security_Asgnt_wk12.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
  _next = next;
       _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
  try
     {
    await _next(context);
       }
          catch (Exception ex)
         {
    _logger.LogError(ex, "An unhandled exception occurred");
     await HandleExceptionAsync(context, ex);
     }
        }

 private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
            var code = HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new 
   { 
     message = "An error occurred while processing your request. Please try again later.",
                error = exception.Message // Remove in production
    });

   context.Response.ContentType = "application/json";
       context.Response.StatusCode = (int)code;
     return context.Response.WriteAsync(result);
  }
  }
}
