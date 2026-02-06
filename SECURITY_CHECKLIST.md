# Security Implementation Checklist (Annex A)

## Project: Ace Job Agency Membership System

---

## 1. Registration Form (4%)

| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| First Name field | ? | Implemented in RegisterDto with validation |
| Last Name field | ? | Implemented in RegisterDto with validation |
| Gender field | ? | Dropdown with Male/Female/Other options |
| NRIC field (encrypted) | ? | Encrypted using AES-256 before saving to database |
| Email address (unique) | ? | Unique constraint in database, duplicate check in controller |
| Password field | ? | Password input with strength requirements |
| Confirm Password field | ? | Matches password field validation |
| Date of Birth | ? | Date picker with 18+ age validation |
| Resume upload (.docx/.pdf) | ? | File validation and secure storage |
| Who Am I field (special chars) | ? | HTML encoded, allows all special characters |
| Photo upload (optional) | ? | Optional image upload with validation |
| Store in database | ? | All data saved to Members table |
| Duplicate email check | ? | Server-side validation before registration |

---

## 2. Securing Credentials (10%)

### Strong Password
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Minimum 12 characters | ? | Validated in PasswordService.ValidatePasswordStrength() |
| Uppercase letters | ? | Regex validation |
| Lowercase letters | ? | Regex validation |
| Numbers | ? | Regex validation |
| Special characters | ? | Regex validation using pattern |
| User feedback on weak password | ? | Clear error messages returned to user |
| Client-side validation | ? | HTML5 validation + JavaScript |
| Server-side validation | ? | PasswordService validation |

### Password Protection
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Password hashing | ? | BCrypt with 12 rounds |
| Salt generation | ? | BCrypt automatically generates salt |
| Secure storage | ? | Only hashed password stored in database |

---

## 3. Securing User Data & Passwords (6%)

| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| NRIC encryption | ? | AES-256 encryption using EncryptionService |
| Password hashing | ? | BCrypt hashing |
| Data encryption at rest | ? | NRIC stored encrypted in database |
| Decryption for display | ? | NRIC decrypted only when displaying on homepage |
| Secure key management | ? | Encryption key in appsettings.json (move to env vars in prod) |

---

## 4. Session Management (10%)

| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Session creation on login | ? | SessionService.CreateSessionAsync() |
| Session timeout (30 min) | ? | Configured in Program.cs |
| Session validation | ? | SessionValidationMiddleware |
| Session extension on activity | ? | ExtendSessionAsync() called on each request |
| Multiple login detection | ? | Track sessions per user with IP and User Agent |
| Session hijacking prevention | ? | Validate IP address and User Agent |
| Secure session cookies | ? | HttpOnly, Secure, SameSite=Strict |
| Session cleanup on logout | ? | InvalidateSessionAsync() |

---

## 5. Login/Logout (10%)

### Credential Verification
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Login after registration | ? | User can login only with registered credentials |
| Email validation | ? | Check if user exists in database |
| Password verification | ? | BCrypt.Verify() |
| Redirect after login | ? | Redirect to profile.html |
| Display user info on homepage | ? | Profile page shows decrypted NRIC and member data |

### Rate Limiting
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Track failed login attempts | ? | FailedLoginAttempts field in Member model |
| Account lockout after 3 failures | ? | Lock account after 3 consecutive failures |
| Lockout duration (15 min) | ? | LockedOutUntil set to UTC+15 minutes |
| Automatic recovery | ? | Account unlocks automatically after 15 minutes |
| Reset counter on success | ? | FailedLoginAttempts reset to 0 on successful login |

### Logout
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Clear session | ? | HttpContext.Session.Clear() |
| Invalidate session | ? | SessionService.InvalidateSessionAsync() |
| Redirect to login | ? | Frontend redirects to login.html |

### Audit Logging
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Log user activities | ? | AuditService logs all actions |
| Store in database | ? | AuditLogs table |
| Track login/logout | ? | LOGIN_SUCCESS, LOGOUT events |
| Track failed attempts | ? | LOGIN_FAILED_* events |
| IP address logging | ? | Captured in audit logs |
| User agent logging | ? | Captured in audit logs |
| Timestamp | ? | UTC timestamp for all events |

---

## 6. Anti-Bot (Captcha) (5%)

| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Google reCAPTCHA integration | ? | reCAPTCHA v3 implemented |
| Captcha on registration | ? | Required field in RegisterDto |
| Captcha on login | ? | Required field in LoginDto |
| Server-side verification | ? | CaptchaService.ValidateCaptchaAsync() |
| Error handling | ? | Returns error if captcha fails |

---

## 7. Input Validation (15%)

### Injection Prevention
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| SQL Injection prevention | ? | Entity Framework parameterized queries |
| XSS prevention | ? | HTML encoding using WebUtility.HtmlEncode() |
| CSRF protection | ? | Antiforgery tokens configured |
| Input sanitization | ? | SanitizeInput() method removes dangerous characters |

### Validation Checks
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Email format validation | ? | [EmailAddress] attribute + regex |
| NRIC format validation | ? | Regex pattern for Singapore NRIC |
| Date validation | ? | Age check (18+) |
| File type validation | ? | AllowedResumeExtensions, AllowedImageExtensions |
| File size validation | ? | Max 5MB per file |
| Client-side validation | ? | HTML5 required, pattern attributes |
| Server-side validation | ? | DataAnnotations + custom validation |
| Error messages | ? | Clear, user-friendly error messages |
| Proper encoding | ? | HTML encode before saving to database |

---

## 8. Proper Error Handling (5%)

| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Custom error messages | ? | User-friendly messages throughout application |
| 404 error handling | ? | GlobalExceptionHandlerMiddleware |
| 403 error handling | ? | Unauthorized responses |
| 500 error handling | ? | Global exception handler |
| Graceful error pages | ? | Error displayed in UI |
| Error logging | ? | ILogger used throughout |

---

## 9. Advanced Features (10%)

### Account Policies and Recovery
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| Automatic recovery after lockout | ? | Check LockedOutUntil timestamp |
| Password history | ? | Store last 2 passwords in PasswordHistory |
| Prevent password reuse | ? | CheckPasswordHistory() in PasswordService |
| Change password feature | ? | /api/member/change-password endpoint |
| Minimum password age | ? | Cannot change within 5 minutes |
| Maximum password age | ? | Must change after 90 days |
| Password age enforcement | ? | Check LastPasswordChangedDate on login |

---

## 10. Additional Security Measures

### Security Headers
| Header | Status | Value |
|--------|--------|-------|
| X-Content-Type-Options | ? | nosniff |
| X-Frame-Options | ? | DENY |
| X-XSS-Protection | ? | 1; mode=block |
| Referrer-Policy | ? | strict-origin-when-cross-origin |
| Content-Security-Policy | ? | Configured |
| Remove Server header | ? | Removed in middleware |

### HTTPS
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| HTTPS redirect | ? | UseHttpsRedirection() in Program.cs |
| Secure cookies | ? | Cookie.SecurePolicy = Always |
| HSTS | ? | UseHsts() in production |

### File Upload Security
| Requirement | Status | Implementation Details |
|-------------|--------|------------------------|
| File type validation | ? | Whitelist of allowed extensions |
| File size limit | ? | 5MB maximum |
| Secure file naming | ? | GUID + sanitized names |
| Separate upload directory | ? | wwwroot/uploads/{resumes,photos} |
| File storage outside webroot | ?? | Consider for production |

---

## Testing & Code Analysis (5%)

### GitHub Security Scanning
| Tool | Status | Notes |
|------|--------|-------|
| GitHub Advanced Security | ?? | To be performed |
| Dependabot alerts | ?? | Monitor for vulnerable dependencies |
| Code scanning | ?? | GitHub CodeQL analysis |
| Secret scanning | ?? | Check for exposed secrets |

### Recommended Tests
- [x] SQL Injection attempts
- [x] XSS payload injection
- [x] CSRF token validation
- [x] Session hijacking attempts
- [x] Brute force login attempts
- [x] File upload vulnerabilities
- [x] Password policy enforcement
- [x] Session timeout verification
- [x] Account lockout mechanism

---

## Demo Checklist (5%)

### Demo Requirements
- [ ] Working prototype with database
- [ ] Error-free demonstration
- [ ] Show all security features
- [ ] Demonstrate according to checklist
- [ ] 5-7 minute demo duration

### Demo Flow
1. Registration
   - Show validation
   - Show strong password requirement
   - Upload resume and photo
   - Complete CAPTCHA
   
2. Login
   - Successful login
   - Show failed attempts (lockout)
   - Complete CAPTCHA
   
3. Profile Page
   - Show decrypted NRIC
   - Show member information
   
4. Change Password
   - Show password policy
   - Show password history check

5. Security Features
   - Show session management
   - Show audit logs
 - Show account recovery

---

## Report (10%)

### Report Contents
- [ ] Introduction
- [ ] Security features implemented
- [ ] Technical architecture
- [ ] Implementation details
- [ ] Testing results
- [ ] Screenshots
- [ ] Code snippets
- [ ] Challenges faced
- [ ] Security recommendations
- [ ] Conclusion

---

## Summary

**Total Implementation Coverage: ~95%**

### Completed Features (90%+)
- ? Registration with all required fields
- ? Strong password policies
- ? NRIC encryption
- ? Session management
- ? Login/Logout with rate limiting
- ? CAPTCHA integration
- ? Input validation & injection prevention
- ? Error handling
- ? Advanced account policies
- ? Security headers
- ? Audit logging
- ? File upload security

### Recommendations for Production
- Move encryption keys to environment variables
- Implement proper logging (Application Insights/Serilog)
- Use Redis for distributed sessions
- Store files in cloud storage (Azure Blob)
- Enable GitHub Advanced Security scanning
- Add comprehensive unit tests
- Implement rate limiting middleware
- Add email verification
- Consider 2FA implementation

---

**Date:** [Current Date]  
**Prepared by:** [Your Name]  
**Module:** Application Security  
**Module Group:** [Your Group]
