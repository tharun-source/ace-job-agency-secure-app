namespace Application_Security_Asgnt_wk12.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Relaxed CSP for development - allows inline scripts and localhost
            context.Response.Headers.Add("Content-Security-Policy", 
                "default-src 'self' 'unsafe-inline' 'unsafe-eval' https://localhost:* http://localhost:* data: blob:; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://localhost:* http://localhost:*; " +
                "style-src 'self' 'unsafe-inline' https://localhost:* http://localhost:*; " +
                "img-src 'self' data: https: http:; " +
                "font-src 'self' data:; " +
                "connect-src 'self' https://localhost:* http://localhost:* ws://localhost:* wss://localhost:*;");

            await _next(context);
        }
    }
}
