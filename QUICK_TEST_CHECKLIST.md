# ? QUICK TESTING CHECKLIST

## ?? **BEFORE YOU START**

```bash
# 1. Build the application
dotnet build

# 2. Update database (if needed)
dotnet ef database update

# 3. Run the application
dotnet run

# 4. Open browser
# https://localhost:7134
```

---

## ? **15-MINUTE QUICK TEST**

### **1. REGISTRATION (3 minutes)**
```
? Go to https://localhost:7134/register.html
? Fill form with test data:
  - Name: Test User
  - Email: test@example.com
  - Password: TestPass123!@#
  - NRIC: S1234567D
  - DOB: 01/01/1990
  - Upload resume (.pdf)
  - Upload photo (.jpg) - optional
? Click Register
? ? Should succeed and redirect to login
```

### **2. LOGIN (2 minutes)**
```
? Use credentials from registration
? Check reCAPTCHA box
? Click Login
? ? Should redirect to profile page
```

### **3. PROFILE (2 minutes)**
```
? Verify all data displays correctly
? Check timestamp shows "SGT"
? Click "Download Resume"
? ? File should download
```

### **4. EDIT PROFILE (2 minutes)**
```
? Click "Edit Profile"
? Change first name to "Updated Test"
? Click "Update Profile"
? ? Should see success message
? ? Profile should show updated name
```

### **5. CHANGE PASSWORD (2 minutes)**
```
? Click "Change Password"
? Enter:
  - Current: TestPass123!@#
  - New: NewTestPass123!@#
  - Confirm: NewTestPass123!@#
? Click "Change Password"
? ? Should succeed
```

### **6. AUDIT LOGS (2 minutes)**
```
? Click "View Audit Logs"
? Verify you see:
  - REGISTER_SUCCESS
  - LOGIN_SUCCESS
  - UPDATE_PROFILE_SUCCESS
  - CHANGE_PASSWORD_SUCCESS
  - DOWNLOAD_RESUME
? Click "Export to CSV"
? ? CSV file should download
```

### **7. PASSWORD RESET (3 minutes)**
```
? Logout
? Click "Forgot Password?"
? Enter: test@example.com
? Click "Send Reset Link"
? Check browser CONSOLE (F12 ? Console)
? Copy reset link from console
? Paste link in browser
? Enter new password: ResetPass123!@#
? Click "Reset Password"
? ? Should succeed
? Login with new password
? ? Should work
```

### **8. LOGOUT & RE-LOGIN (1 minute)**
```
? Click "Logout"
? Login again with latest password
? ? Should work
```

---

## ? **SECURITY TESTS (5 minutes)**

### **XSS Test**
```
? Edit profile
? In "About Me": <script>alert('XSS')</script>
? Save
? View profile
? ? Should show encoded text, NO alert popup
```

### **SQL Injection Test**
```
? Logout
? Login page
? Email: ' OR '1'='1
? Password: anything
? ? Should fail with "Invalid email or password"
```

### **Account Lockout Test**
```
? Create new account
? Try login with wrong password 3 times
? ? Should lock account after 3 attempts
? ? Should show lockout message
```

### **Weak Password Test**
```
? Try to register with password: "weak"
? ? Should show error about password requirements
```

---

## ? **CRITICAL CHECKS**

### **Homepage**
```
? Icons display correctly (no "??")
? Purple gradient background
? "Not logged in" banner when logged out
? "Logged in as..." banner when logged in
? All feature cards visible when logged in
```

### **Profile Page**
```
? All personal info displays
? NRIC shows decrypted (S1234567D)
? Date format: DD/MM/YYYY
? Time format: includes "SGT"
? File download links work
? All buttons work
```

### **Audit Logs**
```
? All actions are logged
? Timestamps show "SGT"
? Color-coded badges (green/red/blue)
? CSV export works
? CSV includes all columns
```

### **Database**
```
? Open SQL Server Object Explorer
? View Members table
? ? PasswordHash starts with $2a$12$
? ? NRICEncrypted is long string
? ? No plain text passwords visible
```

---

## ?? **TEST RESULTS**

### **Passed: __ / 25**

| Test | Result | Notes |
|------|--------|-------|
| Registration | ? PASS ? FAIL | |
| Login | ? PASS ? FAIL | |
| Profile Display | ? PASS ? FAIL | |
| Edit Profile | ? PASS ? FAIL | |
| Change Password | ? PASS ? FAIL | |
| Password Reset | ? PASS ? FAIL | |
| Audit Logs | ? PASS ? FAIL | |
| File Download | ? PASS ? FAIL | |
| XSS Prevention | ? PASS ? FAIL | |
| SQL Injection Prevention | ? PASS ? FAIL | |
| Account Lockout | ? PASS ? FAIL | |
| Session Timeout | ? PASS ? FAIL | |
| Password History | ? PASS ? FAIL | |
| Weak Password Rejection | ? PASS ? FAIL | |
| Duplicate Email Check | ? PASS ? FAIL | |
| Age Validation | ? PASS ? FAIL | |
| File Type Validation | ? PASS ? FAIL | |
| reCAPTCHA | ? PASS ? FAIL | |
| Logout | ? PASS ? FAIL | |
| Re-login | ? PASS ? FAIL | |
| Homepage Navigation | ? PASS ? FAIL | |
| Timezone Display | ? PASS ? FAIL | |
| NRIC Encryption | ? PASS ? FAIL | |
| Password Hashing | ? PASS ? FAIL | |
| CSV Export | ? PASS ? FAIL | |

---

## ?? **COMMON ISSUES & FIXES**

### **Issue: Application won't start**
```
Fix:
1. Check port 7134 isn't in use
2. Run: dotnet clean
3. Run: dotnet build
4. Run: dotnet run
```

### **Issue: Database error**
```
Fix:
1. Run: dotnet ef database drop -f
2. Run: dotnet ef database update
3. Re-run application
```

### **Issue: reCAPTCHA invalid**
```
Fix:
1. Check appsettings.json has correct keys
2. Or test without reCAPTCHA (comment out validation)
```

### **Issue: File upload fails**
```
Fix:
1. Check wwwroot/uploads/ folder exists
2. Check file size < 5MB (resume) or < 2MB (photo)
3. Check file type is allowed
```

### **Issue: Session timeout immediate**
```
Fix:
1. Check Program.cs line 19
2. Should be: TimeSpan.FromMinutes(30)
3. NOT: TimeSpan.FromSeconds(30)
```

---

## ?? **SCREENSHOTS TO TAKE**

```
? 1. Homepage (logged out)
? 2. Homepage (logged in)
? 3. Registration form
? 4. Registration success
? 5. Login page
? 6. Profile page (full view)
? 7. Edit profile page
? 8. Change password success
? 9. Audit logs table
? 10. CSV export file (in Excel)
? 11. Password reset console output
? 12. Password reset success
? 13. Account lockout message
? 14. XSS test result (encoded text)
? 15. SQL injection test result (error message)
? 16. Database view (Members table)
? 17. Database view (AuditLogs table)
? 18. Database view (UserSessions table)
```

---

## ?? **TESTING PRIORITY**

### **MUST TEST (Critical):**
1. ? Registration
2. ? Login
3. ? Profile display
4. ? Password hashing (check database)
5. ? NRIC encryption (check database)
6. ? Audit logging
7. ? XSS prevention
8. ? SQL injection prevention

### **SHOULD TEST (Important):**
9. ? Edit profile
10. ? Change password
11. ? Password reset
12. ? Account lockout
13. ? File upload/download
14. ? Session timeout
15. ? Password history

### **NICE TO TEST (Additional):**
16. ? CSV export
17. ? Multiple browsers
18. ? Mobile responsive
19. ? Timezone display
20. ? Token expiration

---

## ?? **TIME ESTIMATES**

```
Quick Test (Core Features):     15 minutes
Full Test (All Features):       45 minutes
Security Testing:       15 minutes
Database Verification:      10 minutes
Screenshot Documentation:       20 minutes
?????????????????????????????????????????
Total Comprehensive Testing:  ~2 hours
```

---

## ?? **READY TO START!**

1. ? Build application
2. ? Run application
3. ? Open browser to https://localhost:7134
4. ? Follow checklist from top to bottom
5. ? Take screenshots
6. ? Document results

**Good luck! ??**

---

## ?? **NEED HELP?**

If tests fail, check:
1. **COMPLETE_TESTING_GUIDE.md** - Detailed instructions
2. **Browser console** (F12) - JavaScript errors
3. **Application console** - Server errors
4. **Database** - Data verification

**Your application is well-built and should pass all tests! ??**
