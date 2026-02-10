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

app.UseStaticFiles();

// Configure 404 status code pages
app.UseStatusCodePagesWithReExecute("/404.html");

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseSession();
app.UseMiddleware<SessionValidationMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Redirect root to index page
app.MapGet("/", () => Results.Redirect("/index.html"));

// Fallback for any unmatched routes
app.MapFallback(() => Results.Redirect("/404.html"));

app.Run();
