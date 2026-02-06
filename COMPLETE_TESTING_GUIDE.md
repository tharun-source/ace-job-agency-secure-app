# ?? COMPLETE TESTING GUIDE - Step by Step

## ?? **QUICK START - Testing Your Application**

---

## ?? **PRE-TESTING CHECKLIST**

### **Before You Start:**
- [ ] Application builds successfully (`dotnet build`)
- [ ] Database exists (run `dotnet ef database update` if needed)
- [ ] reCAPTCHA keys configured (or use test mode)
- [ ] Port 7134 is available

---

## ?? **TESTING WORKFLOW**

### **Step 1: Start the Application**

```bash
cd "Application Security Asgnt wk12"
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7134
      Now listening on: http://localhost:5134
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**? Test: Application Starts**
- Open browser: https://localhost:7134
- Should see homepage with "Ace Job Agency"

---

## ?? **TEST SUITE 1: HOMEPAGE**

### **Test 1.1: Homepage Loads**
```
URL: https://localhost:7134
Expected: 
  ? Homepage displays
  ? Icons show correctly (no "??")
  ? Purple gradient background
  ? "Welcome to Our Secure Platform" section visible
```

### **Test 1.2: Not Logged In State**
```
Expected to see:
  ? Yellow banner: "You are not logged in"
  ? "Register" card visible
  ? "Login" card visible
  ? "My Profile" card NOT visible
  ? "Edit Profile" card NOT visible
  ? "Change Password" card NOT visible
  ? "Audit Logs" card NOT visible
```

**Screenshot:** Take screenshot of homepage (not logged in)

---

## ?? **TEST SUITE 2: REGISTRATION**

### **Test 2.1: Navigate to Registration**
```
Action: Click "Register" card or "Get Started ?" button
URL: https://localhost:7134/register.html
Expected:
  ? Registration form loads
  ? All fields visible
  ? reCAPTCHA widget shows (or checkbox)
```

### **Test 2.2: Client-Side Validation**

#### **A. Empty Form Submission**
```
Action: Click "Register" without filling anything
Expected:
  ? Browser validation errors appear
  ? "Please fill out this field" messages
  ? Form does NOT submit
```

#### **B. Invalid Email Format**
```
Test Data:
  Email: "notanemail"
  
Expected:
  ? "Please enter a valid email address" error
```

#### **C. Weak Password**
```
Test Data:
  Password: "weak"
  
Expected:
  ? Error: "Password must be at least 12 characters"
```

### **Test 2.3: Successful Registration**

**Test Data:**
```
First Name:      John
Last Name:    Doe
Gender:          Male
NRIC: S1234567D
Email:    john.doe.test@example.com
Date of Birth:   01/01/1990 (must be 18+)
Password:SecurePass123!@#
Confirm Password: SecurePass123!@#
Resume:          Upload a .pdf or .docx file
Photo:  Upload a .jpg or .png file (optional)
Who Am I:     "Test user for security application"
reCAPTCHA:       Check the box
```

**Action:** Fill all fields and click "Register"

**Expected:**
```
? Success message: "Registration successful! You can now login."
? Redirects to login page after 2 seconds
? Check console for any errors (F12 ? Console)
```

**Screenshot:** Registration success message

### **Test 2.4: Duplicate Email**
```
Action: Try to register again with same email
Expected:
  ? Error: "Email address is already registered."
```

### **Test 2.5: Password Requirements**

Test each requirement individually:

```
A. Too short (< 12 chars):
   Password: "Short1!"
   Expected: ? Error about minimum length

B. No uppercase:
   Password: "lowercase123!"
   Expected: ? Error about uppercase letter

C. No lowercase:
   Password: "UPPERCASE123!"
   Expected: ? Error about lowercase letter

D. No numbers:
   Password: "NoNumbers!@#"
   Expected: ? Error about numbers

E. No special characters:
   Password: "NoSpecial123"
   Expected: ? Error about special characters
```

### **Test 2.6: File Upload Validation**

```
A. Invalid resume format:
   Upload: .txt or .exe file
   Expected: ? Error about file type

B. Invalid photo format:
   Upload: .txt or .pdf file
   Expected: ? Error about image file type

C. File too large:
   Upload: >5MB resume or >2MB photo
   Expected: ? Error about file size
```

### **Test 2.7: Age Validation**
```
Test Data:
  Date of Birth: Today's date (under 18)
  
Expected:
  ? Error: "You must be at least 18 years old to register."
```

---

## ?? **TEST SUITE 3: LOGIN**

### **Test 3.1: Navigate to Login**
```
URL: https://localhost:7134/login.html
Expected:
  ? Login form displays
  ? "Forgot Password?" link visible
  ? reCAPTCHA widget visible
```

### **Test 3.2: Invalid Credentials**

#### **A. Wrong Email**
```
Test Data:
  Email: nonexistent@example.com
  Password: AnyPassword123!
  
Expected:
  ? Error: "Invalid email or password."
  ? No specific indication which is wrong (security)
```

#### **B. Wrong Password**
```
Test Data:
  Email: john.doe.test@example.com (registered email)
  Password: WrongPassword123!
  
Expected:
  ? Error: "Invalid email or password."
  ? Failed login attempts increased in database
```

### **Test 3.3: Account Lockout**

```
Action: Try to login with wrong password 3 times

Attempt 1:
  Expected: ? Error: "Invalid email or password."

Attempt 2:
  Expected: ? Error: "Invalid email or password."

Attempt 3:
  Expected: ? Error: "Account locked due to multiple failed login attempts. 
            Please try again after 15 minutes."

Attempt 4 (immediately):
  Expected: ? Error: "Account is locked. Please try again after X minutes."
```

**Screenshot:** Account locked message

**Wait Time Test:**
- Wait 15 minutes OR manually update database
- Try login again with correct password
- Expected: ? Login succeeds (lockout expired)

### **Test 3.4: Successful Login**

**IMPORTANT:** If account is locked, wait 15 minutes or create new account

```
Test Data:
  Email: john.doe.test@example.com
  Password: SecurePass123!@#
  reCAPTCHA: Check the box
  
Action: Click "Login"

Expected:
  ? Success message briefly appears
  ? Redirects to profile page
  ? URL changes to /profile.html
```

**Screenshot:** Login success

---

## ?? **TEST SUITE 4: PROFILE PAGE**

### **Test 4.1: Profile Data Display**

```
Expected to see:
  ? "Welcome to Your Profile" heading
  ? Personal Information section:
      - Name: John Doe
  - Email: john.doe.test@example.com
      - Gender: Male
 - NRIC: S1234567D (decrypted)
 - Date of Birth: 01/01/1990
      - Member Since: Today's date
  - Last Login: Current timestamp with "SGT"
      - About Me: "Test user for security application"
  
  ? Files section:
      - Resume: Download Resume link
      - Photo: Download Photo link (if uploaded)
  
  ? Buttons visible:
  - ?? Home
      - Edit Profile
  - Change Password
      - View Audit Logs
      - Logout
```

**Screenshot:** Profile page with all data

### **Test 4.2: Time Display**
```
Check Last Login timestamp:
  Expected format: DD/MM/YYYY, HH:MM AM/PM SGT
  Example: 23/01/2026, 02:30 PM SGT
  
? Verify timezone shows "SGT"
? Verify date format is DD/MM/YYYY (not MM/DD/YYYY)
```

### **Test 4.3: File Downloads**

#### **A. Download Resume**
```
Action: Click "Download Resume" link
Expected:
  ? File downloads
  ? File opens correctly
  ? Audit log created (check later)
```

#### **B. Download Photo**
```
Action: Click "Download Photo" link (if available)
Expected:
  ? Image downloads
  ? Image displays correctly
  ? Audit log created
```

### **Test 4.4: Navigation**

#### **A. Home Button**
```
Action: Click "?? Home" button
Expected:
  ? Returns to homepage
  ? NOW shows logged-in state
  ? Green banner: "Logged in as John Doe"
  ? All 6 feature cards visible (Profile, Edit, Password, Logs)
```

#### **B. Back to Home Link**
```
Action: Click "? Back to Home" at top
Expected:
  ? Same as Home button test
```

---

## ?? **TEST SUITE 5: EDIT PROFILE**

### **Test 5.1: Navigate to Edit Profile**
```
Action: Click "Edit Profile" button from profile page
URL: https://localhost:7134/edit-profile.html
Expected:
  ? Form loads with current data pre-filled
```

### **Test 5.2: Update Basic Information**

```
Test Data:
  First Name: John Updated
  Last Name: Doe Updated
  Gender: Change to Female
  Date of Birth: 02/02/1991
  About Me: "Updated profile information"
  
Action: Click "Update Profile" (without changing files)

Expected:
  ? Success message: "Profile updated successfully!"
  ? Redirects to profile page
  ? Changes reflected on profile page
```

**Screenshot:** Updated profile

### **Test 5.3: Re-upload Files**

#### **A. Update Resume Only**
```
Action: 
  1. Keep all fields as is
  2. Upload new resume file
  3. Click "Update Profile"
  
Expected:
  ? New resume uploaded
  ? Old resume deleted
  ? Profile updated
```

#### **B. Update Photo Only**
```
Action: 
  1. Keep all fields as is
  2. Upload new photo file
  3. Click "Update Profile"
  
Expected:
  ? New photo uploaded
  ? Old photo deleted
  ? Profile updated
```

### **Test 5.4: Validation on Edit**
```
A. Under 18 years old:
   Date of Birth: Recent date
   Expected: ? Error: "You must be at least 18 years old."

B. Empty required fields:
   First Name: (empty)
   Expected: ? Validation error
```

---

## ?? **TEST SUITE 6: CHANGE PASSWORD**

### **Test 6.1: Navigate to Change Password**
```
Action: Click "Change Password" from profile
URL: https://localhost:7134/change-password.html
Expected:
  ? Form displays with 3 password fields
```

### **Test 6.2: Validation Tests**

#### **A. Wrong Current Password**
```
Test Data:
  Current Password: WrongPassword123!
  New Password: NewSecurePass123!@#
  Confirm: NewSecurePass123!@#
  
Expected:
  ? Error: "Current password is incorrect."
```

#### **B. New Password Same as Current**
```
Test Data:
  Current Password: SecurePass123!@#
  New Password: SecurePass123!@#
  Confirm: SecurePass123!@#
  
Expected:
  ? Error: "New password must be different from current password."
```

#### **C. Passwords Don't Match**
```
Test Data:
Current Password: SecurePass123!@#
  New Password: NewSecurePass123!@#
  Confirm: DifferentPass123!@#
  
Expected:
  ? Error: "Passwords do not match."
```

#### **D. Weak New Password**
```
Test Data:
  Current Password: SecurePass123!@#
  New Password: weak
  Confirm: weak
  
Expected:
  ? Error about password requirements
```

### **Test 6.3: Successful Password Change**

```
Test Data:
  Current Password: SecurePass123!@#
  New Password: NewSecurePass123!@#
  Confirm: NewSecurePass123!@#
  
Action: Click "Change Password"

Expected:
  ? Success message: "Password changed successfully!"
  ? Redirects to profile page
  ? Audit log created
```

**Screenshot:** Password change success

### **Test 6.4: Verify New Password Works**
```
Action:
  1. Logout
  2. Login with NEW password
  
Test Data:
  Email: john.doe.test@example.com
  Password: NewSecurePass123!@#
  
Expected:
  ? Login succeeds
```

### **Test 6.5: Password History Test**

```
Action: Try to change password back to OLD password

Test Data:
  Current Password: NewSecurePass123!@#
  New Password: SecurePass123!@# (the old one)
  Confirm: SecurePass123!@#
  
Expected:
  ? Error: "You cannot reuse any of your last 2 passwords."
```

**Screenshot:** Password history error

### **Test 6.6: Minimum Password Age**

```
Action: Try to change password immediately after changing it

Expected:
  ? Error: "You must wait at least 5 minutes before changing 
          your password again."
```

---

## ?? **TEST SUITE 7: PASSWORD RESET**

### **Test 7.1: Forgot Password Flow**

#### **A. Request Reset Link**
```
Action:
  1. Logout (if logged in)
  2. Go to login page
  3. Click "Forgot Password?" link
  
URL: https://localhost:7134/forgot-password.html

Test Data:
  Email: john.doe.test@example.com
  
Action: Click "Send Reset Link"

Expected:
  ? Success message appears
  ? Check CONSOLE for reset link (development mode)
```

**Screenshot:** Forgot password success + console output

#### **B. Console Output Should Show:**
```
========== PASSWORD RESET EMAIL ==========
To: john.doe.test@example.com
Subject: Password Reset Request
Reset Link: https://localhost:7134/reset-password.html?token=xxx&email=yyy
Link expires in 15 minutes
==========================================
```

**Copy the entire reset link from console!**

### **Test 7.2: Reset Password with Token**

```
Action:
  1. Copy reset link from console
  2. Paste in browser OR manually navigate to reset-password.html
  3. Enter new password
  
Test Data:
  New Password: ResetPassword123!@#
  Confirm: ResetPassword123!@#
  
Action: Click "Reset Password"

Expected:
  ? Success message: "Password has been reset successfully..."
  ? Redirects to login page after 3 seconds
```

**Screenshot:** Password reset success

### **Test 7.3: Login with Reset Password**
```
Test Data:
  Email: john.doe.test@example.com
  Password: ResetPassword123!@#
  
Expected:
  ? Login succeeds
```

### **Test 7.4: Token Expiration**

```
Action:
  1. Request reset link
  2. Wait 16 minutes (or manually test)
  3. Try to use the link
  
Expected:
  ? Error: "Reset token has expired. Please request a new one."
```

### **Test 7.5: Token Reuse**
```
Action:
  1. Successfully reset password
  2. Try to use same reset link again
  
Expected:
  ? Error: Invalid or expired token
```

### **Test 7.6: Email Enumeration Protection**
```
Test Data:
  Email: nonexistent@example.com
  
Expected:
  ? Same success message (doesn't reveal if email exists)
? No reset link in console for non-existent email
```

---

## ?? **TEST SUITE 8: AUDIT LOGS**

### **Test 8.1: View Audit Logs**
```
Action: Click "View Audit Logs" from profile
URL: https://localhost:7134/audit-logs.html

Expected:
? Table displays with all your activities
  ? Columns: Timestamp (Singapore Time), Action, IP Address, Details
  ? Actions include:
      - REGISTER_SUCCESS
      - LOGIN_SUCCESS
      - LOGIN_FAILED_INVALID_PASSWORD (if you tried wrong password)
      - ACCOUNT_LOCKED (if you triggered lockout)
      - CHANGE_PASSWORD_SUCCESS
      - DOWNLOAD_RESUME
      - DOWNLOAD_PHOTO
      - UPDATE_PROFILE_SUCCESS
 - LOGOUT
      - FORGOT_PASSWORD_SUCCESS
      - RESET_PASSWORD_SUCCESS
```

**Screenshot:** Audit logs table

### **Test 8.2: Timestamp Format**
```
Check timestamps in table:
  Expected format: DD/MM/YYYY, HH:MM:SS AM/PM SGT
  Example: 23/01/2026, 02:30:45 PM SGT
  
? All timestamps show "SGT"
? All dates in DD/MM/YYYY format
```

### **Test 8.3: Export to CSV**
```
Action: Click "Export to CSV" button

Expected:
  ? CSV file downloads
  ? Filename: audit-logs-YYYY-MM-DD.csv
  ? File opens in Excel/text editor
  ? Contains all columns and data
  ? Timestamps include SGT label
```

**Screenshot:** CSV file content

### **Test 8.4: Action Color Coding**
```
Check badge colors:
  ? Green badges: SUCCESS actions
  ? Red badges: FAILED, ERROR, LOCKED actions
  ? Blue badges: INFO actions (LOGOUT, DOWNLOAD, etc.)
```

---

## ?? **TEST SUITE 9: SESSION MANAGEMENT**

### **Test 9.1: Session Timeout**

**Setup:** Change timeout to 2 minutes for testing

**In Program.cs, change line 19:**
```csharp
options.IdleTimeout = TimeSpan.FromMinutes(2); // Was 30
```

```
Action:
  1. Login successfully
  2. Wait 2 minutes without any activity
3. Try to access profile or any protected page
  
Expected:
  ? Redirects to login page
  ? Error message about session timeout (may vary)
```

**Remember to change back to 30 minutes after testing!**

### **Test 9.2: Multiple Logins**

```
Action:
  1. Login in Browser 1 (e.g., Chrome)
  2. Open Browser 2 (e.g., Firefox or Incognito)
  3. Login with same account in Browser 2
  
Expected:
  ? Both sessions work independently
  ? Each creates separate session in database
```

**Verify in Database:**
```sql
SELECT * FROM UserSessions WHERE MemberId = 1;
-- Should see multiple active sessions
```

### **Test 9.3: Session Validation**

```
Action:
  1. Login successfully
  2. Open browser dev tools (F12)
  3. Application/Storage ? Cookies
  4. Manually delete the session cookie
  5. Try to access profile
  
Expected:
  ? Redirects to login (no valid session)
```

---

## ?? **TEST SUITE 10: SECURITY TESTS**

### **Test 10.1: XSS Prevention**

#### **A. Script in "Who Am I" field**
```
Test Data (Registration or Edit Profile):
  Who Am I: <script>alert('XSS')</script>
  
Expected:
  ? Text is HTML encoded in database
  ? Displays as plain text on profile (no script execution)
  ? Shows: &lt;script&gt;alert('XSS')&lt;/script&gt;
```

#### **B. Script in Name Fields**
```
Test Data:
  First Name: <script>hack()</script>
  
Expected:
  ? Dangerous characters removed
  ? Displays as: hack() or scripthack()script
```

**Screenshot:** XSS attempt sanitized

### **Test 10.2: SQL Injection Prevention**

```
Test Data (Login):
  Email: ' OR '1'='1
  Password: anything
  
Expected:
  ? Login fails
  ? No database error
  ? Error: "Invalid email or password."
```

### **Test 10.3: File Upload Security**

```
A. Malicious filename:
   Filename: ../../etc/passwd.pdf
   Expected: ? Sanitized filename in uploads folder

B. Executable disguised as PDF:
   Filename: virus.exe renamed to virus.pdf
   Expected: ? File type validation catches it

C. Path traversal:
   Filename: ../../../malicious.pdf
   Expected: ? Saved safely in designated folder
```

### **Test 10.4: HTTPS Enforcement**

```
Action: Try to access http://localhost:5134
Expected:
  ? Automatically redirects to https://localhost:7134
```

### **Test 10.5: CSRF Protection**

```
This is automatically tested by the framework
All POST requests require valid anti-forgery token
```

---

## ?? **TEST SUITE 11: DATABASE VERIFICATION**

### **Test 11.1: View Database in Visual Studio**

```
Steps:
  1. View ? SQL Server Object Explorer
  2. Expand: (localdb)\MSSQLLocalDB ? Databases ? AceJobAgencyDB
  3. Expand: Tables
  
Check each table:
```

#### **A. Members Table**
```sql
SELECT 
    Id, FirstName, LastName, Email, 
    LEFT(PasswordHash, 20) + '...' AS PasswordHash,
 LEFT(NRICEncrypted, 20) + '...' AS NRICEncrypted,
    FailedLoginAttempts, LockedOutUntil, 
    LastPasswordChangedDate
FROM Members;
```

**Verify:**
- ? PasswordHash is BCrypt format ($2a$12$...)
- ? NRICEncrypted is long encrypted string
- ? FailedLoginAttempts resets after successful login
- ? LockedOutUntil is NULL (or past time if unlocked)

#### **B. AuditLogs Table**
```sql
SELECT TOP 20 
    MemberId, Action, IpAddress, Timestamp, Details
FROM AuditLogs
ORDER BY Timestamp DESC;
```

**Verify:**
- ? All your actions are logged
- ? Timestamps are in UTC (database)
- ? IP addresses recorded (::1 for localhost)

#### **C. UserSessions Table**
```sql
SELECT 
    Id, MemberId, SessionId, CreatedAt, ExpiresAt, 
    IsActive, IpAddress
FROM UserSessions
WHERE MemberId = 1
ORDER BY CreatedAt DESC;
```

**Verify:**
- ? Sessions created on login
- ? ExpiresAt is 30 minutes after CreatedAt
- ? IsActive set to false on logout

---

## ?? **TEST SUITE 12: EDGE CASES**

### **Test 12.1: Browser Compatibility**
```
Test in multiple browsers:
  ? Chrome
  ? Firefox
  ? Edge
  ? Safari (if available)
  
Expected: ? Works identically in all
```

### **Test 12.2: Mobile Responsiveness**
```
Action:
1. Open browser dev tools (F12)
  2. Toggle device toolbar (Ctrl+Shift+M)
  3. Test different screen sizes
  
Expected:
  ? Responsive design
  ? All buttons accessible
  ? Forms usable on small screens
```

### **Test 12.3: Concurrent Sessions**
```
Action:
  1. Login in 2 different browsers
  2. Change password in Browser 1
  3. Try to use Browser 2
  
Expected:
  ? Browser 2 session still valid (or requires re-login)
```

### **Test 12.4: Network Errors**
```
Action:
  1. Open dev tools ? Network tab
  2. Enable offline mode
  3. Try to login
  
Expected:
  ? Graceful error message
  ? No application crash
```

---

## ?? **TESTING CHECKLIST SUMMARY**

### **Critical Tests (Must Pass):**
```
? Registration works
? Login works
? Session management works
? Password change works
? Password reset works
? Profile display works
? File uploads work
? File downloads work
? Audit logs work
? Account lockout works
? XSS prevention works
? SQL injection prevention works
```

### **Important Tests (Should Pass):**
```
? Edit profile works
? Session timeout works
? Password history works
? Minimum password age works
? Token expiration works
? Email enumeration protection works
? Multiple sessions work
? Timezone displays correctly (SGT)
? CSV export works
```

### **Nice-to-Have Tests:**
```
? Mobile responsive
? Browser compatibility
? Network error handling
? Concurrent session management
```

---

## ?? **TEST RESULTS TEMPLATE**

Create a document with your test results:

```
# Test Results - [Date]

## Test Environment
- OS: Windows 11
- Browser: Chrome 120
- .NET Version: 8.0
- Database: LocalDB

## Test Results

### Registration
? PASS - User can register successfully
? PASS - Duplicate email rejected
? PASS - Password validation works
? PASS - File upload works
? PASS - Age validation works

### Login
? PASS - Login with valid credentials works
? PASS - Invalid credentials rejected
? PASS - Account locks after 3 failed attempts
? PASS - Account unlocks after 15 minutes

### Profile Management
? PASS - Profile displays all data correctly
? PASS - Edit profile works
? PASS - File re-upload works
? PASS - Timestamps show Singapore Time

### Password Management
? PASS - Change password works
? PASS - Password reset works
? PASS - Password history prevents reuse
? PASS - Token expiration works

### Security
? PASS - XSS prevention works
? PASS - SQL injection prevention works
? PASS - HTTPS enforced
? PASS - Input validation works

### Audit Logging
? PASS - All actions logged
? PASS - CSV export works
? PASS - Timestamps correct

## Issues Found
None - all tests passed!

## Screenshots
[Attach screenshots for each major feature]
```

---

## ?? **QUICK TEST SCRIPT**

If you want to test quickly, follow this 15-minute script:

```
1. Start application (1 min)
2. Register new user (2 min)
3. Login (1 min)
4. View profile (1 min)
5. Edit profile (2 min)
6. Change password (2 min)
7. View audit logs (1 min)
8. Export CSV (1 min)
9. Test password reset (3 min)
10. Logout and re-login (1 min)

Total: ~15 minutes for basic testing
```

---

## ?? **SCREENSHOTS NEEDED FOR REPORT**

```
Essential Screenshots:
1. Homepage (not logged in)
2. Registration form
3. Registration success
4. Login page
5. Profile page with data
6. Edit profile page
7. Change password success
8. Audit logs table
9. Password reset email (console)
10. Password reset success
11. Account lockout message
12. Database view (Members table with hashed passwords)
13. Security header test (optional)
```

---

## ?? **YOU'RE READY TO TEST!**

**Start with Test Suite 1 (Homepage) and work your way through systematically.**

**Good luck with your testing! ??**
