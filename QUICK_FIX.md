# ?? REGISTRATION FAILED - Quick Fix Guide

## ?? Step 1: Find the Error (30 seconds)

### In Your Browser:
1. Press **F12**
2. Click **Network** tab
3. Click **Register** button again
4. Find `/api/auth/register` (will be red if error)
5. Click on it
6. Click **Response** tab
7. **Read the error message**

---

## ?? Common Errors & Instant Fixes

### Error: "Invalid CAPTCHA. Please try again."

**Cause:** reCAPTCHA v3 validation failed

**Instant Fix:**
1. Clear browser cache (Ctrl + Shift + Delete)
2. Close ALL browser windows
3. Reopen browser
4. Try again in **Incognito mode** (Ctrl + Shift + N)

**If still failing:**
```powershell
# Restart application:
# In Visual Studio: Shift+F5 then F5
```

---

### Error: "Email address is already registered."

**Fix:**
Use a different email address like:
- `test123@example.com`
- `newuser@example.com`  
- `demo456@example.com`

---

### Error: "Password does not meet requirements"

**Fix:**
Use this password: `SecurePass123!`

Must have:
- 12+ characters ?
- Uppercase ?
- Lowercase ?
- Numbers ?
- Special chars ?

---

### Error: "Database connection error" or "An error occurred"

**Fix:**
```powershell
# Run in Terminal:
cd "Application Security Asgnt wk12"
dotnet ef database update
```

---

### Error: "File upload error"

**Fix:**
```powershell
# Create upload folders:
mkdir -Force wwwroot\uploads\resumes
mkdir -Force wwwroot\uploads\photos
```

---

### Error: "Failed to fetch" (in Console)

**Cause:** Application not running or wrong URL

**Fix:**
1. Check Visual Studio shows: `Now listening on: https://localhost:7134`
2. If not, restart application (F5)

---

## ? Quick Test Data

Copy-paste this (change email if needed):

```
First Name: Test
Last Name: User
Gender: Male
NRIC: S1234567Z
Email: testuser789@example.com
Password: SecurePass123!
Confirm Password: SecurePass123!
Date of Birth: 01/01/1995
Resume: [Upload any .pdf file]
Who Am I: I am testing the application
```

---

## ?? Nuclear Option (If Nothing Works)

```powershell
# 1. Stop application (Shift+F5)

# 2. Navigate to project
cd "Application Security Asgnt wk12"

# 3. Drop and recreate database
dotnet ef database drop -f
dotnet ef database update

# 4. Clean and rebuild
dotnet clean
dotnet build

# 5. Restart application
dotnet run
```

Then try registering again with a new email.

---

## ?? Check Application Logs

### In Visual Studio Output Window:

Look for lines like:
```
info: ... CAPTCHA validation successful
info: ... Register successful
```

Or errors like:
```
error: ... CAPTCHA validation failed
error: ... Database error
```

---

## ?? Most Likely Issue: reCAPTCHA

Since you just updated to v3, try:

1. **Clear ALL browser data:**
   - Ctrl + Shift + Delete
   - Select "All time"
   - Check all boxes
   - Clear data
   - Close browser
   - Reopen

2. **Try Incognito:**
   - Ctrl + Shift + N
   - Go to: `https://localhost:7134/register.html`
   - Accept security warning
   - Try registering

3. **Verify reCAPTCHA:**
   - Should see small badge in bottom-right corner
   - Should say "reCAPTCHA" with "Privacy - Terms"
   - Should NOT show checkbox or error

---

## ?? What to Share if Still Broken

1. Screenshot of Network ? Response tab
2. Screenshot of Console tab errors
3. Visual Studio Output window text
4. The email you're trying to use

---

## ? 30-Second Fix

Try this first:

```powershell
# Stop app (Shift+F5)
# Run this:
cd "Application Security Asgnt wk12"
dotnet ef database update
mkdir -Force wwwroot\uploads\resumes
mkdir -Force wwwroot\uploads\photos
# Start app (F5)
# Try in Incognito mode with email: test999@example.com
```

---

**Which error are you seeing in Network ? Response?** That will tell us exactly what to fix!
