# Testing Guide for Ace Job Agency Application

## Overview
This guide provides comprehensive testing scenarios to validate all security features implemented in the application.

---

## 1. Registration Testing

### Test Case 1.1: Successful Registration
**Steps:**
1. Navigate to `https://localhost:PORT/register.html`
2. Fill all required fields with valid data:
   - First Name: John
   - Last Name: Doe
   - Gender: Male
   - NRIC: S1234567A
   - Email: john.doe@test.com
   - Password: SecurePass123!
   - Confirm Password: SecurePass123!
   - Date of Birth: 01/01/1990
   - Resume: Upload valid .pdf or .docx file
   - Photo: Upload valid image (optional)
   - Who Am I: "I am a software developer"
3. Complete CAPTCHA
4. Click Register

**Expected Result:**
- ? Registration successful message
- ? Redirect to login page
- ? User data saved in database with encrypted NRIC
- ? Files uploaded to server

### Test Case 1.2: Duplicate Email
**Steps:**
1. Register with email: john.doe@test.com
2. Try to register again with same email

**Expected Result:**
- ? Error: "Email address is already registered"

### Test Case 1.3: Weak Password
**Steps:**
Try registering with these passwords:
- "short" 
- "onlylowercase"
- "ONLYUPPERCASE"
- "NoNumbers!"
- "NoSpecial123"
- "Short1!"

**Expected Result:**
- ? Clear error messages for each validation failure

### Test Case 1.4: Invalid NRIC Format
**Steps:**
Try registering with invalid NRIC:
- "12345678"
- "A1234567B"
- "S123456A"

**Expected Result:**
- ? Error: "Invalid NRIC format"

### Test Case 1.5: Age Validation
**Steps:**
1. Set Date of Birth to current year (under 18)

**Expected Result:**
- ? Error: "You must be at least 18 years old"

### Test Case 1.6: File Upload Validation
**Steps:**
Try uploading:
- .exe file as resume
- .txt file as resume
- File larger than 5MB
- .pdf file as photo

**Expected Result:**
- ? Appropriate error messages for invalid files

### Test Case 1.7: Missing CAPTCHA
**Steps:**
1. Fill all fields
2. Don't complete CAPTCHA
3. Submit form

**Expected Result:**
- ? Error: "Please complete the CAPTCHA"

### Test Case 1.8: XSS Prevention
**Steps:**
1. Try entering in "Who Am I" field:
   ```
 <script>alert('XSS')</script>
   <img src=x onerror=alert('XSS')>
   ```

**Expected Result:**
- ? Text saved but HTML encoded
- ? No script execution on display

### Test Case 1.9: SQL Injection Prevention
**Steps:**
Try entering in email field:
```
' OR '1'='1
admin'--
' UNION SELECT * FROM Members--
```

**Expected Result:**
- ? Input sanitized or rejected
- ? No database errors
- ? No SQL injection successful

---

## 2. Login Testing

### Test Case 2.1: Successful Login
**Steps:**
1. Navigate to `https://localhost:PORT/login.html`
2. Enter valid credentials
3. Complete CAPTCHA
4. Click Login

**Expected Result:**
- ? Login successful message
- ? Session created
- ? Redirect to profile page
- ? Audit log entry created
- ? LastLoginDate updated

### Test Case 2.2: Invalid Credentials
**Steps:**
1. Enter valid email
2. Enter wrong password
3. Complete CAPTCHA
4. Click Login

**Expected Result:**
- ? Error: "Invalid email or password"
- ? FailedLoginAttempts incremented
- ? Audit log entry created

### Test Case 2.3: Account Lockout
**Steps:**
1. Enter valid email
2. Enter wrong password 3 times
3. Try to login with correct password

**Expected Result:**
- ? After 3rd attempt: "Account locked due to multiple failed login attempts"
- ? Cannot login even with correct password
- ? LockedOutUntil timestamp set to 15 minutes from now
- ? Audit log shows ACCOUNT_LOCKED event

### Test Case 2.4: Account Recovery After Lockout
**Steps:**
1. Lock account (3 failed attempts)
2. Wait 15 minutes (or manually change LockedOutUntil in database)
3. Try to login with correct credentials

**Expected Result:**
- ? Login successful
- ? FailedLoginAttempts reset to 0
- ? LockedOutUntil cleared

### Test Case 2.5: Missing CAPTCHA
**Steps:**
1. Fill credentials
2. Don't complete CAPTCHA
3. Submit

**Expected Result:**
- ? Error: "Please complete the CAPTCHA"

---

## 3. Session Management Testing

### Test Case 3.1: Session Creation
**Steps:**
1. Login successfully
2. Check browser developer tools ? Application ? Cookies

**Expected Result:**
- ? Session cookie created
- ? Cookie has HttpOnly flag
- ? Cookie has Secure flag
- ? Cookie has SameSite=Strict

### Test Case 3.2: Session Timeout
**Steps:**
1. Login successfully
2. Wait 30 minutes (or manually expire session in database)
3. Try to access profile page

**Expected Result:**
- ? Session expired
- ? Redirect to login page or 401 error

### Test Case 3.3: Session Hijacking Prevention
**Steps:**
1. Login from Browser A
2. Copy session cookie
3. Paste session cookie in Browser B (different User-Agent)
4. Try to access protected resource

**Expected Result:**
- ? Session invalidated due to User-Agent mismatch
- ? 401 Unauthorized error

### Test Case 3.4: Multiple Device Login
**Steps:**
1. Login from Computer A
2. Login from Computer B
3. Check database for active sessions

**Expected Result:**
- ? Two separate sessions created
- ? Both sessions tracked in UserSessions table
- ? Different SessionIds for each device

---

## 4. Profile Page Testing

### Test Case 4.1: View Profile
**Steps:**
1. Login successfully
2. Navigate to profile page

**Expected Result:**
- ? All member information displayed
- ? NRIC displayed in decrypted form
- ? Last login date shown
- ? Proper HTML encoding for "Who Am I" text

### Test Case 4.2: Unauthorized Access
**Steps:**
1. Without logging in, navigate to profile page

**Expected Result:**
- ? Redirect to login page or 401 error

---

## 5. Change Password Testing

### Test Case 5.1: Successful Password Change
**Steps:**
1. Login
2. Go to change password
3. Enter current password
4. Enter new strong password
5. Confirm new password
6. Submit

**Expected Result:**
- ? Password changed successfully
- ? Old password added to history
- ? LastPasswordChangedDate updated
- ? Can login with new password

### Test Case 5.2: Wrong Current Password
**Steps:**
1. Enter incorrect current password
2. Enter new password
3. Submit

**Expected Result:**
- ? Error: "Current password is incorrect"

### Test Case 5.3: Password Reuse Prevention
**Steps:**
1. Change password to NewPass123!
2. Change password to AnotherPass123!
3. Try to change password back to NewPass123!

**Expected Result:**
- ? Error: "You cannot reuse any of your last 2 passwords"

### Test Case 5.4: Minimum Password Age
**Steps:**
1. Change password
2. Immediately try to change password again (within 5 minutes)

**Expected Result:**
- ? Error: "You must wait at least 5 minutes before changing your password again"

### Test Case 5.5: Maximum Password Age
**Steps:**
1. Manually set LastPasswordChangedDate to 91 days ago in database
2. Try to login

**Expected Result:**
- ? Login blocked
- ? Message: "Your password has expired. Please reset your password."

### Test Case 5.6: Same as Current Password
**Steps:**
1. Try to change password to same as current password

**Expected Result:**
- ? Error: "New password must be different from current password"

---

## 6. Logout Testing

### Test Case 6.1: Successful Logout
**Steps:**
1. Login
2. Click Logout

**Expected Result:**
- ? Session cleared
- ? Session invalidated in database
- ? Redirect to login page
- ? Audit log entry created
- ? Cannot access profile page after logout

---

## 7. File Upload Testing

### Test Case 7.1: Valid Resume Upload
**Steps:**
1. Upload .pdf file (< 5MB)
2. Upload .docx file (< 5MB)

**Expected Result:**
- ? Files uploaded successfully
- ? Files stored in wwwroot/uploads/resumes/
- ? Filename sanitized (GUID added)

### Test Case 7.2: Invalid File Type
**Steps:**
Try uploading:
- .exe file
- .bat file
- .php file
- .txt file

**Expected Result:**
- ? Error: "Only .pdf and .docx files are allowed"

### Test Case 7.3: File Size Limit
**Steps:**
1. Try to upload file larger than 5MB

**Expected Result:**
- ? Error: "File size exceeds 5MB limit"

### Test Case 7.4: Photo Upload
**Steps:**
1. Upload .jpg, .jpeg, .png, .gif files

**Expected Result:**
- ? Files uploaded successfully
- ? Stored in wwwroot/uploads/photos/

---

## 8. Security Headers Testing

### Test Case 8.1: Verify Security Headers
**Steps:**
1. Open developer tools ? Network tab
2. Make any request to the application
3. Check response headers

**Expected Result:**
- ? X-Content-Type-Options: nosniff
- ? X-Frame-Options: DENY
- ? X-XSS-Protection: 1; mode=block
- ? Referrer-Policy: strict-origin-when-cross-origin
- ? Content-Security-Policy: present
- ? Server header: removed

---

## 9. Audit Logging Testing

### Test Case 9.1: Verify Audit Logs
**Steps:**
1. Perform various actions (register, login, logout, change password)
2. Query AuditLogs table in database

**Expected Result:**
- ? All actions logged
- ? Timestamp recorded
- ? IP address captured
- ? User agent captured
- ? Action type recorded

---

## 10. Error Handling Testing

### Test Case 10.1: 404 Error
**Steps:**
1. Navigate to non-existent page

**Expected Result:**
- ? Custom error message displayed
- ? No stack trace exposed

### Test Case 10.2: 500 Error
**Steps:**
1. Trigger server error (e.g., stop database)
2. Try to perform database operation

**Expected Result:**
- ? User-friendly error message
- ? Error logged
- ? No sensitive information exposed

---

## 11. HTTPS Testing

### Test Case 11.1: HTTPS Redirect
**Steps:**
1. Try to access application via HTTP

**Expected Result:**
- ? Automatically redirected to HTTPS

---

## 12. Database Testing

### Test Case 12.1: Verify Encrypted Data
**Steps:**
1. Register a user with NRIC: S1234567A
2. Query Members table in database
3. Check NRICEncrypted field

**Expected Result:**
- ? NRIC stored in encrypted form (Base64 string)
- ? Not readable as plain text

### Test Case 12.2: Verify Password Hash
**Steps:**
1. Register with password: MyPassword123!
2. Query Members table
3. Check PasswordHash field

**Expected Result:**
- ? Password stored as BCrypt hash (starts with $2a$ or $2b$)
- ? Not stored in plain text

---

## Test Data

### Valid Test Accounts

**Account 1:**
- Email: john.doe@test.com
- Password: SecurePass123!
- NRIC: S1234567A

**Account 2:**
- Email: jane.smith@test.com
- Password: AnotherPass123!
- NRIC: S9876543B

**Account 3 (For Lockout Testing):**
- Email: locked.user@test.com
- Password: LockedPass123!
- NRIC: S5555555C

### Invalid Test Data

**Weak Passwords:**
- "short"
- "onlylowercase"
- "ONLYUPPERCASE"
- "NoNumbers!"
- "NoSpecial123"

**XSS Payloads:**
```
<script>alert('XSS')</script>
<img src=x onerror=alert('XSS')>
"><script>alert(String.fromCharCode(88,83,83))</script>
```

**SQL Injection Payloads:**
```
' OR '1'='1
admin'--
' UNION SELECT * FROM Members--
1; DROP TABLE Members--
```

---

## Automated Testing with Tools

### Recommended Security Testing Tools

1. **OWASP ZAP** - Web application security scanner
2. **Burp Suite** - Web vulnerability scanner
3. **Postman** - API testing
4. **GitHub Advanced Security** - Code scanning

### How to Use OWASP ZAP

1. Download and install OWASP ZAP
2. Configure browser to use ZAP as proxy
3. Navigate through application
4. Run automated scan
5. Review findings
6. Fix vulnerabilities
7. Re-test

---

## Test Results Template

```
Test Case: [ID]
Test Date: [DATE]
Tester: [NAME]
Status: [PASS/FAIL]
Notes: [OBSERVATIONS]
Screenshots: [ATTACH IF APPLICABLE]
```

---

## Summary Checklist

- [ ] All registration tests passed
- [ ] All login tests passed
- [ ] Session management validated
- [ ] Profile page secure
- [ ] Password change working
- [ ] Logout functioning
- [ ] File uploads secure
- [ ] Security headers present
- [ ] Audit logs working
- [ ] Error handling graceful
- [ ] HTTPS enforced
- [ ] Data encrypted in database
- [ ] No XSS vulnerabilities
- [ ] No SQL injection vulnerabilities
- [ ] GitHub security scan clean

---

**Remember:** Always test in a safe environment. Never test on production systems.

Good luck with your testing! ??
