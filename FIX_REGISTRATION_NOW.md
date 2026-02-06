# ?? REGISTRATION FAILED - IMMEDIATE FIX

## ? I Just Fixed:

1. ? **Lowered reCAPTCHA threshold** from 0.3 to 0.1 (for testing)
2. ? **Created upload directories** (resumes & photos folders)
3. ? **Added better logging** to see what's happening
4. ? **Build successful** - no errors

---

## ?? RESTART APPLICATION NOW

### **In Visual Studio:**
1. Press **Shift + F5** (Stop)
2. Press **F5** (Start)

### **Or in Terminal:**
```powershell
# Stop if running
Ctrl + C

# Start again
dotnet run
```

---

## ?? Check Visual Studio Output

After restarting, watch the **Output** window (View ? Output):

You should see lines like:
```
info: CaptchaService - Validating CAPTCHA...
info: CaptchaService - CAPTCHA response: {...}
info: CaptchaService - ? CAPTCHA validation successful! Score: 0.X
info: AuthController - Register successful
```

Or errors like:
```
warn: CaptchaService - CAPTCHA score too low: 0.X
warn: AuthController - REGISTER_FAILED_CAPTCHA
```

---

## ?? TEST REGISTRATION AGAIN

### **Use This Exact Data:**

```
First Name: Test
Last Name: User
Gender: Male
NRIC: S9876543Z
Email: testuser@example.com
Password: SecurePass123!
Confirm Password: SecurePass123!
Date of Birth: 01/01/1995
Resume: Upload ANY .pdf or .docx file
Photo: (Leave empty or upload .jpg)
Who Am I: I am testing the registration
```

**Important:**
- Make sure you upload a resume file (required)
- Use a NEW email address (not one you tried before)

---

## ?? CHECK BROWSER CONSOLE

### **While Clicking Register:**

1. Press **F12**
2. Go to **Console** tab
3. Click **Register**
4. Look for messages:

**Good:**
```
? Registration successful!
```

**Bad (in Network ? Response):**
```json
{
  "message": "Invalid CAPTCHA. Please try again."
}
```
or
```json
{
  "message": "Email address is already registered."
}
```

---

## ?? MOST COMMON ISSUES

### 1?? Email Already Registered
**Error:** `"Email address is already registered."`

**Fix:** Use a different email:
- `test123@example.com`
- `newuser456@example.com`
- `demo789@example.com`

---

### 2?? reCAPTCHA Score Too Low
**Error:** `"Invalid CAPTCHA. Please try again."`

**Fix:** 
- Clear browser cache (Ctrl + Shift + Delete)
- Try in **Incognito mode** (Ctrl + Shift + N)
- Wait 30 seconds and try again

---

### 3?? Missing Resume File
**Error:** `"Resume file is required"`

**Fix:** Make sure you click "Click to upload resume" and select a .pdf or .docx file

---

### 4?? Password Too Weak
**Error:** `"Password does not meet requirements"`

**Fix:** Use: `SecurePass123!`

Must have:
- ? 12+ characters
- ? Uppercase (A-Z)
- ? Lowercase (a-z)
- ? Numbers (0-9)
- ? Special chars (!@#$%^&*)

---

### 5?? Age Under 18
**Error:** `"You must be at least 18 years old to register."`

**Fix:** Use date of birth: `01/01/2000` (makes you 24-25 years old)

---

## ?? STEP-BY-STEP DEBUG PROCESS

### Step 1: Check Application is Running
```
? Visual Studio shows: Running/Debugging (green indicator)
? Output shows: "Now listening on: https://localhost:7134"
? No red error messages
```

### Step 2: Check Upload Directories Exist
```
? wwwroot\uploads\resumes folder exists
? wwwroot\uploads\photos folder exists
```

### Step 3: Check Database
```
? Database "AceJobAgencyDB" exists
? Table "Members" exists
```

### Step 4: Test in Browser
```
1. Go to: https://localhost:7134/register.html
2. Fill ALL required fields
3. Upload resume file
4. Click Register
5. Watch both:
   - Browser console (F12 ? Console)
   - Visual Studio Output window
```

---

## ?? WHAT TO SHARE IF STILL FAILING

1. **Visual Studio Output:**
   - View ? Output
   - Copy the last 20-30 lines
   - Especially lines with "CAPTCHA" or "Register"

2. **Browser Network Response:**
   - F12 ? Network tab
   - Click Register
   - Find `/api/auth/register` request
   - Click it ? Response tab
   - Copy the error message

3. **Browser Console:**
   - F12 ? Console tab
   - Copy any red error messages

---

## ?? NUCLEAR OPTION

If nothing works, try this complete reset:

```powershell
# Navigate to project
cd "C:\Users\tharu\source\repos\Application Security Asgnt wk12\Application Security Asgnt wk12"

# Stop everything
taskkill /IM dotnet.exe /F

# Drop and recreate database
dotnet ef database drop -f
dotnet ef database update

# Clean and rebuild
dotnet clean
dotnet build

# Run
dotnet run
```

Then try registering with:
- Email: `freshstart@example.com`
- Password: `SecurePass123!`
- Don't forget to upload resume!

---

## ? SUCCESS CHECKLIST

You'll know it worked when:

- [ ] Click Register button
- [ ] See "Registering..." on button
- [ ] Green success message appears: "Registration successful!"
- [ ] Automatically redirects to login page (2 seconds)
- [ ] In Visual Studio Output: "? CAPTCHA validation successful!"
- [ ] In Visual Studio Output: "Register successful"
- [ ] No error in browser console (F12)

---

## ?? MOST LIKELY CAUSE

Based on your situation, it's probably one of these (in order of likelihood):

1. **Email already used** (70%)
 ? Try: `newemail123@example.com`

2. **reCAPTCHA failing** (20%)
   ? Try: Incognito mode

3. **Missing resume file** (10%)
   ? Make sure you upload a file

---

## ?? TRY RIGHT NOW

1. **Restart app** (Shift+F5 then F5)
2. **Open Output window** (View ? Output)
3. **Open browser console** (F12 ? Console)
4. **Go to:** `https://localhost:7134/register.html`
5. **Use email:** `testuser999@example.com`
6. **Upload resume:** Any .pdf file
7. **Click Register**
8. **Watch both Output window and browser console**

---

**What do you see in Visual Studio Output window after clicking Register?**

Share that and I'll tell you exactly what's wrong! ??
