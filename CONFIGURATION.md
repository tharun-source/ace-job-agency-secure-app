# Configuration Steps

## 1. Update appsettings.json

Replace these placeholders in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AceJobAgencyDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    // ?? Update if using different SQL Server
  },
  "ReCaptcha": {
    "SiteKey": "YOUR_RECAPTCHA_SITE_KEY",        // ?? REQUIRED: Get from Google reCAPTCHA
    "SecretKey": "YOUR_RECAPTCHA_SECRET_KEY"     // ?? REQUIRED: Get from Google reCAPTCHA
  },
  "EncryptionKey": "YourSecure32CharacterKeyHere!!!"  // ?? RECOMMENDED: Change for production
}
```

## 2. Get Google reCAPTCHA Keys

1. Go to: https://www.google.com/recaptcha/admin
2. Click "+" to add a new site
3. Fill in:
   - **Label:** Ace Job Agency
 - **reCAPTCHA type:** Score based (v3)
   - **Domains:** localhost (for development)
4. Accept terms and click "Submit"
5. Copy your **Site Key** and **Secret Key**

## 3. Update HTML Files

### File: wwwroot/register.html

**Line ~262:** Update the data-sitekey
```html
<!-- BEFORE -->
<div class="g-recaptcha" data-sitekey="YOUR_RECAPTCHA_SITE_KEY"></div>

<!-- AFTER -->
<div class="g-recaptcha" data-sitekey="6LcXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"></div>
```

**Line ~313:** Update the API URL
```javascript
// BEFORE
const response = await fetch('https://localhost:7xxx/api/auth/register', {

// AFTER (check your actual port in Properties/launchSettings.json)
const response = await fetch('https://localhost:7001/api/auth/register', {
```

### File: wwwroot/login.html

**Line ~130:** Update the data-sitekey
```html
<!-- BEFORE -->
<div class="g-recaptcha" data-sitekey="YOUR_RECAPTCHA_SITE_KEY"></div>

<!-- AFTER -->
<div class="g-recaptcha" data-sitekey="6LcXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"></div>
```

**Line ~147:** Update the API URL
```javascript
// BEFORE
const response = await fetch('https://localhost:7xxx/api/auth/login', {

// AFTER
const response = await fetch('https://localhost:7001/api/auth/login', {
```

### File: wwwroot/profile.html

**Line ~88:** Update the API URL
```javascript
// BEFORE
const response = await fetch('https://localhost:7xxx/api/member/profile', {

// AFTER
const response = await fetch('https://localhost:7001/api/member/profile', {
```

**Line ~143:** Update the API URL
```javascript
// BEFORE
const response = await fetch('https://localhost:7xxx/api/auth/logout', {

// AFTER
const response = await fetch('https://localhost:7001/api/auth/logout', {
```

## 4. Find Your Port Number

Check `Properties/launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7001;http://localhost:5001",  // ?? Your port is here
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

The HTTPS port (e.g., 7001) is what you need to use in your HTML files.

## 5. Database Configuration (Optional)

If you want to use a different database:

### Using SQL Server (not LocalDB)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=AceJobAgencyDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
}
```

### Using Azure SQL Database
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Database=AceJobAgencyDB;User Id=youradmin;Password=yourpassword;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

## 6. Environment Variables (Production)

For production, use environment variables instead of appsettings.json:

### Windows
```powershell
$env:ReCaptcha__SiteKey = "your-site-key"
$env:ReCaptcha__SecretKey = "your-secret-key"
$env:EncryptionKey = "your-32-char-key"
```

### Linux/Mac
```bash
export ReCaptcha__SiteKey="your-site-key"
export ReCaptcha__SecretKey="your-secret-key"
export EncryptionKey="your-32-char-key"
```

### Azure App Service
1. Go to Azure Portal
2. Open your App Service
3. Settings ? Configuration ? Application settings
4. Add:
   - Name: `ReCaptcha__SiteKey`, Value: `your-site-key`
   - Name: `ReCaptcha__SecretKey`, Value: `your-secret-key`
   - Name: `EncryptionKey`, Value: `your-32-char-key`

## 7. CORS Configuration (if needed)

If your frontend is on a different domain, update `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
    .WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
  .AllowAnyMethod()
    .AllowAnyHeader()
          .AllowCredentials());
});
```

## 8. Verification Checklist

After configuration, verify:

- [ ] appsettings.json has valid reCAPTCHA keys
- [ ] All HTML files have correct site key
- [ ] All HTML files have correct API port
- [ ] Database connection string is correct
- [ ] Application runs without errors
- [ ] Can access registration page
- [ ] Can access login page
- [ ] reCAPTCHA widget appears on forms

## 9. Common Issues

### Issue: reCAPTCHA not showing
**Solution:** Check that the site key in HTML matches the one from Google

### Issue: "Invalid CAPTCHA" error
**Solution:** Check that the secret key in appsettings.json matches the one from Google

### Issue: Cannot connect to API
**Solution:** Verify the port number in HTML files matches your actual port

### Issue: CORS error
**Solution:** Update CORS policy in Program.cs to include your frontend domain

### Issue: Database connection error
**Solution:** Check connection string and ensure SQL Server is running

## 10. Quick Test

After configuration, test:

1. Run the application: `dotnet run`
2. Open browser to `https://localhost:YOUR_PORT/register.html`
3. Verify reCAPTCHA widget appears
4. Fill form and try to register
5. Should see success message or clear error

If everything works, you're ready to proceed! ?

---

**Need Help?**
- Check README.md for detailed documentation
- Review QUICK_SETUP.md for step-by-step guide
- See TROUBLESHOOTING section in README.md
