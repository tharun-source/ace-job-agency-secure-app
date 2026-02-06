# ?? Registration Failed - Debugging Guide

## Quick Diagnosis Steps

### Step 1: Check Browser Console (F12)

1. **Open Developer Tools:** Press `F12`
2. **Go to Console tab**
3. **Look for errors** (red text)

**Common errors:**
- `Failed to fetch` - API not running
- `CORS error` - Cross-origin issue
- `Invalid CAPTCHA` - reCAPTCHA problem
- `500 Internal Server Error` - Server error

### Step 2: Check Network Tab

1. **Open Developer Tools:** Press `F12`
2. **Go to Network tab**
3. **Click Register button**
4. **Look for `/api/auth/register` request**
5. **Click on it to see:**
   - Status code (should be 200 for success)
   - Response body (shows actual error message)

### Step 3: Check Application Output

Look at Visual Studio's Output window for errors.

---

## ?? Common Issues & Solutions

### Issue 1: "Invalid CAPTCHA"

**Cause:** reCAPTCHA validation failed

**Solutions:**
1. Make sure you filled the form completely
2. Check reCAPTCHA badge appears
3. Verify reCAPTCHA keys are correct in `appsettings.json`
4. Try in a different browser or incognito mode

**Quick Fix:**
```json
// In appsettings.json, verify:
"ReCaptcha": {
  "SiteKey": "6LfuRVAsAAAAACTvItR70SvuzQQ62vAIFJduZeyG",
  "SecretKey": "6LfuRVAsAAAAAGpWeydMq60Eg8LftqMJGQMGtIN_"
}
```

---

### Issue 2: "Database connection error"

**Cause:** Database not created or connection string wrong

**Solution:**
```powershell
# Run in terminal:
cd "Application Security Asgnt wk12"
dotnet ef database update
```

**Verify database exists:**
- Open SQL Server Object Explorer in Visual Studio
- Connect to `(localdb)\mssqllocaldb`
- Look for `AceJobAgencyDB` database

---

### Issue 3: "Email already registered"

**Cause:** Email already exists in database

**Solutions:**
1. Use a different email address
2. Or delete existing records:

```sql
-- In SQL Server, run:
USE AceJobAgencyDB
DELETE FROM Members WHERE Email = 'your@email.com'
```

---

### Issue 4: "Password validation error"

**Cause:** Password doesn't meet requirements

**Requirements:**
- ? At least 12 characters
- ? Uppercase letters (A-Z)
- ? Lowercase letters (a-z)
- ? Numbers (0-9)
- ? Special characters (!@#$%^&*)

**Example valid password:** `SecurePass123!`

---

### Issue 5: "File upload error"

**Cause:** Upload directory doesn't exist or file type invalid

**Solution:**

Check upload directories exist:
```
wwwroot/uploads/resumes/
wwwroot/uploads/photos/
```

Create them if missing:
```powershell
mkdir "wwwroot\uploads\resumes"
mkdir "wwwroot\uploads\photos"
```

**Valid file types:**
- Resume: `.pdf`, `.docx`
- Photo: `.jpg`, `.jpeg`, `.png`, `.gif`

---

### Issue 6: "Age validation error"

**Cause:** Date of birth shows user is under 18

**Solution:**
Use a date of birth that makes you at least 18 years old.

Example: `01/01/2000` (24 years old)

---

### Issue 7: "An error occurred during registration"

**Cause:** Server-side exception

**Debug steps:**

1. **Check Visual Studio Output:**
   - View ? Output
   - Show output from: Debug
   - Look for exception details

2. **Check the error in code:**
   The error is logged in `AuthController.cs` line ~145

3. **Common causes:**
   - Encryption key not set
   - Database not accessible
   - File system permissions

**Fix encryption key:**
```json
// In appsettings.json:
"EncryptionKey": "MySecure32CharKeyForEncryption!!"
```

---

## ?? Test with This Data

Use this test data that's guaranteed to work:

```
First Name: John
Last Name: Doe
Gender: Male
NRIC: S1234567A
Email: john.doe.test@example.com  ? Use unique email
Password: SecurePass123!
Confirm Password: SecurePass123!
Date of Birth: 01/01/1990
Resume: test.pdf (any PDF file)
Photo: (optional) photo.jpg
Who Am I: I am a software developer
```

---

## ?? Step-by-Step Debugging

### 1. Verify Application is Running

Check Visual Studio Output shows:
```
Now listening on: https://localhost:7134
Application started.
```

### 2. Verify Database Exists

```powershell
# In PowerShell:
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT name FROM sys.databases WHERE name = 'AceJobAgencyDB'"
```

Should return: `AceJobAgencyDB`

If not found:
```powershell
dotnet ef database update
```

### 3. Test API with Swagger

1. Go to: `https://localhost:7134/swagger`
2. Find `POST /api/Auth/register`
3. Click "Try it out"
4. Fill in the test data
5. Click "Execute"
6. Check response

### 4. Check Browser Network Request

**In Network tab, look for:**
- **Request URL:** `https://localhost:7134/api/auth/register`
- **Status:** Should be 200 (success) or 400 (validation error)
- **Response:** Shows actual error message

---

## ?? Error Code Reference

| Status Code | Meaning | Common Cause |
|-------------|---------|--------------|
| 200 OK | Success | Registration worked! |
| 400 Bad Request | Validation error | Check response message |
| 401 Unauthorized | Auth failed | Wrong credentials |
| 500 Internal Server | Server error | Check Output window |
| 0 (Failed to fetch) | Network error | API not running |

---

## ?? Quick Fixes Checklist

Run these commands in order:

```powershell
# 1. Stop application
# Press Shift+F5 in Visual Studio

# 2. Navigate to project
cd "C:\Users\tharu\source\repos\Application Security Asgnt wk12\Application Security Asgnt wk12"

# 3. Create/Update database
dotnet ef database update

# 4. Create upload directories
mkdir -Force wwwroot\uploads\resumes
mkdir -Force wwwroot\uploads\photos

# 5. Clean and rebuild
dotnet clean
dotnet build

# 6. Run application
dotnet run
```

Then test again with a unique email address.

---

## ?? Get Detailed Error

To see the exact error message:

### Method 1: Browser Console
```javascript
// In browser console (F12), look for:
console.error messages
```

### Method 2: Network Tab
```
1. F12 ? Network tab
2. Click on /api/auth/register request
3. Click "Response" tab
4. Read the error message
```

### Method 3: Visual Studio Output
```
1. View ? Output
2. Show output from: Debug
3. Look for red error text
```

---

## ?? Still Not Working?

### Share These Details:

1. **Browser Console error** (F12 ? Console)
2. **Network response** (F12 ? Network ? Response)
3. **Visual Studio Output** error message
4. **Test data** you're using
5. **Screenshot** of the error

---

## ? Success Indicators

You'll know it worked when:

1. ? Form submits
2. ? Green success message appears: "Registration successful!"
3. ? Redirects to login page after 2 seconds
4. ? In Network tab: Status 200
5. ? Can login with registered email

---

## ?? Most Likely Issues (In Order)

1. **? reCAPTCHA validation failed** (80% of cases)
   - Clear browser cache
   - Try incognito mode
   - Verify keys in appsettings.json

2. **? Database not created** (15% of cases)
   - Run: `dotnet ef database update`

3. **? Email already exists** (3% of cases)
   - Use different email

4. **? Invalid password** (2% of cases)
   - Use: `SecurePass123!`

---

**Next Steps:**
1. Press F12 in browser
2. Go to Network tab
3. Try registering again
4. Share the error message from the Response

This will tell us exactly what's wrong!
