# Quick Setup Guide

## Step 1: Prerequisites
- Visual Studio 2022 or VS Code
- .NET 8.0 SDK
- SQL Server (LocalDB)

## Step 2: Database Setup

1. Open terminal in project directory
2. Run migrations:
```bash
dotnet ef database update
```

## Step 3: Configure reCAPTCHA (Important!)

### Get Your Keys
1. Go to https://www.google.com/recaptcha/admin
2. Register your site
3. Choose reCAPTCHA v3
4. Get your Site Key and Secret Key

### Update Configuration

1. **Update appsettings.json:**
```json
"ReCaptcha": {
  "SiteKey": "YOUR_SITE_KEY_HERE",
  "SecretKey": "YOUR_SECRET_KEY_HERE"
}
```

2. **Update register.html (line ~262):**
```html
<div class="g-recaptcha" data-sitekey="YOUR_SITE_KEY_HERE"></div>
```

3. **Update login.html (line ~130):**
```html
<div class="g-recaptcha" data-sitekey="YOUR_SITE_KEY_HERE"></div>
```

## Step 4: Update API URLs

The HTML files have placeholder URLs (`https://localhost:7xxx`). You need to update them:

### Find Your Port Number
1. Check `Properties/launchSettings.json`
2. Look for `applicationUrl` under `https`
3. Note the port number (e.g., 7001)

### Update Files
Replace `7xxx` with your actual port in:
- `wwwroot/register.html` (line ~313)
- `wwwroot/login.html` (line ~147)
- `wwwroot/profile.html` (line ~88, ~143)

Example:
```javascript
// Change from:
const response = await fetch('https://localhost:7xxx/api/auth/login', {

// To (if your port is 7001):
const response = await fetch('https://localhost:7001/api/auth/login', {
```

## Step 5: Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio

## Step 6: Test the Application

1. Open browser to `https://localhost:YOUR_PORT/register.html`
2. Register a new account
3. Login with your credentials
4. View your profile

## Common Issues

### Issue: Database Error
**Solution:** Run `dotnet ef database update`

### Issue: reCAPTCHA Error
**Solution:** Make sure you've configured the keys correctly

### Issue: Cannot connect to API
**Solution:** Update the port numbers in HTML files

### Issue: Files won't upload
**Solution:** Check that wwwroot/uploads directory exists and has write permissions

## Testing Accounts

For testing, you can create accounts with these formats:

**Example Registration:**
- First Name: John
- Last Name: Doe
- Gender: Male
- NRIC: S1234567A
- Email: john.doe@example.com
- Password: MySecure123!@#
- Date of Birth: 1990-01-01
- Resume: Upload any .pdf or .docx file
- Photo: Optional .jpg/.png file

## Security Testing

### Test Account Lockout
1. Try to login with wrong password 3 times
2. Account will be locked for 15 minutes
3. Wait 15 minutes and try again

### Test Password Policy
Try these passwords to see validation:
- ? "short" - Too short
- ? "alllowercase123!" - No uppercase
- ? "ALLUPPERCASE123!" - No lowercase
- ? "NoNumbers!" - No numbers
- ? "NoSpecialChars123" - No special characters
- ? "MySecure123!@#" - Valid password

### Test Session Timeout
1. Login
2. Wait 30 minutes
3. Try to access profile - should redirect to login

## Next Steps

1. ? Complete registration
2. ? Test all features
3. ? Run GitHub security scanning
4. ? Prepare demo
5. ? Write report

## Support

If you encounter issues:
1. Check the README.md for detailed documentation
2. Review SECURITY_CHECKLIST.md
3. Check application logs
4. Ensure all NuGet packages are restored

Good luck with your demo! ??
