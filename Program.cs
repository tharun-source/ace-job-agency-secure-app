using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Services;
using Application_Security_Asgnt_wk12.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Changed from Seconds to Minutes
    options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
  options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure CORS
builder.Services.AddCors(options =>
{
 options.AddPolicy("AllowSpecificOrigin",
     builder => builder
     .WithOrigins("http://localhost:3000", "https://localhost:3000") // Adjust for your frontend
    .AllowAnyMethod()
          .AllowAnyHeader()
 .AllowCredentials());
});

// Register custom services
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<TwoFactorService>();
builder.Services.AddHttpClient<CaptchaService>();

// Configure Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed errors in development
    app.UseSwagger();
  app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// Use custom middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseHttpsRedirection();

// Configure default files and static files
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("index.html");
app.UseDefaultFiles(defaultFilesOptions);

// Use static files with custom 404 handling
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Let StaticFileMiddleware handle existing files normally
    }
});

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseSession();
app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Redirect root to index page
app.MapGet("/", () => Results.Redirect("/index.html"));

// Custom fallback handler for all unmatched routes
app.Use(async (context, next) =>
{
 await next();
  
    // If nothing handled the request and we got a 404
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
 // Check if this is an API request
        var isApiRequest = context.Request.Path.StartsWithSegments("/api");
    
        if (isApiRequest)
        {
         // Return JSON for API requests
            context.Response.ContentType = "application/json";
   await context.Response.WriteAsync("{\"error\":\"Endpoint not found\",\"statusCode\":404}");
        }
        else
        {
    // Serve the 404.html page for regular requests
    var filePath = Path.Combine(app.Environment.WebRootPath, "404.html");
 if (File.Exists(filePath))
    {
    context.Response.ContentType = "text/html";
           await context.Response.SendFileAsync(filePath);
         }
            else
 {
    // Fallback if 404.html doesn't exist
    context.Response.ContentType = "text/html";
     await context.Response.WriteAsync("<h1>404 - Page Not Found</h1><p>The requested page could not be found.</p>");
   }
     }
    }
});

app.Run();
