# Ace Job Agency - Secure Web Application

## Project Overview
This is a secure membership registration and authentication system for Ace Job Agency, implementing industry-standard security features according to OWASP best practices.

## Features Implemented

### ? **Registration Features** (4%)
- Member registration with comprehensive validation
- Secure storage of member information in database
- Duplicate email detection and prevention
- All required fields as per specification:
  - First Name
  - Last Name
  - Gender
  - NRIC (Encrypted in database)
  - Email (unique)
  - Password
  - Confirm Password
  - Date of Birth
  - Resume (.pdf or .docx file upload)
  - Who Am I (allows all special characters)
  - Photo (optional upload)

### ? **Securing Credentials** (10%)
- **Strong Password Requirements:**
  - Minimum 12 characters
  - Combination of uppercase, lowercase, numbers, and special characters
  - Client-side AND server-side validation
  - User feedback on password strength
- **Password Protection:**
  - BCrypt hashing with salt (12 rounds)
  - Secure password storage

### ? **Securing User Data & Passwords** (6%)
- **Encrypted NRIC:** AES-256 encryption for NRIC data
- **Password Hashing:** BCrypt with salt
- **Encrypted Display:** NRIC decrypted only for homepage display

### ? **Session Management** (10%)
- Secured session creation upon successful login
- Session timeout (30 minutes)
- Session validation middleware
- Multiple login detection from different devices/browsers
- Proper session cleanup on logout
- Session hijacking prevention (IP + User Agent validation)

### ? **Login/Logout** (10%)
- **Credential Verification:**
  - Able to login only after successful registration
  - Email and password validation
- **Rate Limiting:**
  - Account lockout after 3 failed login attempts
  - 15-minute lockout duration with automatic recovery
- **Secure Logout:**
  - Proper session invalidation
  - Clear session data
  - Redirect to login page
- **Audit Logging:**
  - All user activities logged in database
  - Login/logout events tracked
  - Failed attempts recorded

### ? **Anti-Bot (Captcha)** (5%)
- Google reCAPTCHA v3 integration
- Required for both registration and login
- Server-side verification

### ? **Input Validation** (15%)
- **Injection Prevention:**
  - SQL Injection protection (Entity Framework parameterized queries)
  - XSS prevention (input sanitization and HTML encoding)
  - CSRF protection (Antiforgery tokens)
- **Validation:**
  - Email format validation
  - NRIC format validation (Singapore format)
  - Date validation
  - File type validation
  - Client and server-side validation
  - Error messages for invalid inputs
- **Data Encoding:**
  - HTML encoding before saving to database
  - Proper encoding for "Who Am I" field

### ? **Proper Error Handling** (5%)
- **Custom Error Messages:**
  - User-friendly error messages
  - Graceful error handling on all pages
  - Global exception handler middleware
- **Custom Error Pages:**
  - 404, 403, 500 error handling
  - Proper error page display

### ? **Advanced Features** (10%)
- **Account Policies and Recovery:**
  - Automatic account recovery after 15 minutes of lockout
  - Password history (prevents reuse of last 2 passwords)
  - Change password functionality
  - Password age policy (must change after 90 days)
  - Minimum password age (cannot change within 5 minutes)

## Technology Stack

- **Framework:** ASP.NET Core 8.0 (Web API)
- **Database:** SQL Server (LocalDB)
- **ORM:** Entity Framework Core 8.0
- **Password Hashing:** BCrypt.Net-Next
- **Encryption:** AES-256
- **Session Management:** ASP.NET Core Session
- **Frontend:** HTML, CSS, JavaScript
- **CAPTCHA:** Google reCAPTCHA v3

## Security Headers Implemented

The application implements the following security headers:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`
- `Content-Security-Policy` (configured)

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQL Server (LocalDB comes with Visual Studio)
- Google reCAPTCHA keys (for production)

### Installation Steps

1. **Clone/Extract the Project**
   ```bash
   cd "Application Security Asgnt wk12"
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Update Database Connection String** (Optional)
   - Open `appsettings.json`
   - Modify the `ConnectionStrings:DefaultConnection` if needed
   - Default: `Server=(localdb)\\mssqllocaldb;Database=AceJobAgencyDB;Trusted_Connection=True;MultipleActiveResultSets=true`

4. **Configure ReCAPTCHA** (Required for production)
   - Get your keys from https://www.google.com/recaptcha/admin
   - Update `appsettings.json`:
     ```json
     "ReCaptcha": {
 "SiteKey": "YOUR_RECAPTCHA_SITE_KEY",
       "SecretKey": "YOUR_RECAPTCHA_SECRET_KEY"
     }
     ```
   - Update the site key in:
     - `wwwroot/register.html` (line ~262)
     - `wwwroot/login.html` (line ~130)

5. **Create Database**
   ```bash
   dotnet ef database update
   ```

6. **Run the Application**
   ```bash
   dotnet run
   ```
   Or press F5 in Visual Studio

7. **Access the Application**
   - Registration: `https://localhost:7xxx/register.html`
   - Login: `https://localhost:7xxx/login.html`
   - API Docs: `https://localhost:7xxx/swagger`

## Project Structure

```
Application Security Asgnt wk12/
??? Controllers/
?   ??? AuthController.cs          # Registration, Login, Logout
?   ??? MemberController.cs  # Profile, Change Password, Audit Logs
??? Data/
?   ??? ApplicationDbContext.cs    # Database context
??? Middleware/
?   ??? GlobalExceptionHandlerMiddleware.cs
?   ??? SecurityHeadersMiddleware.cs
?   ??? SessionValidationMiddleware.cs
??? Models/
?   ??? AuditLog.cs             # Audit log entity
?   ??? Member.cs      # Member entity
?   ??? Session.cs    # Session entity
?   ??? DTOs/              # Data Transfer Objects
?       ??? RegisterDto.cs
?       ??? LoginDto.cs
?    ??? ChangePasswordDto.cs
?       ??? MemberProfileDto.cs
??? Services/
?   ??? AuditService.cs         # Audit logging
?   ??? CaptchaService.cs         # reCAPTCHA validation
?   ??? EncryptionService.cs      # AES encryption/decryption
?   ??? FileUploadService.cs      # File upload handling
? ??? PasswordService.cs        # Password hashing & validation
?   ??? SessionService.cs     # Session management
??? wwwroot/
?   ??? register.html     # Registration page
?   ??? login.html # Login page
?   ??? profile.html              # Profile page
?   ??? uploads/     # File upload directory
?       ??? resumes/
?       ??? photos/
??? Program.cs    # Application configuration
??? appsettings.json            # Configuration settings
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new member
- `POST /api/auth/login` - Login
- `POST /api/auth/logout` - Logout

### Member
- `GET /api/member/profile` - Get member profile
- `POST /api/member/change-password` - Change password
- `GET /api/member/audit-logs` - Get audit logs

## Database Schema

### Members Table
- Id (PK)
- FirstName
- LastName
- Gender
- NRICEncrypted
- Email (Unique)
- PasswordHash
- DateOfBirth
- ResumePath
- WhoAmI
- PhotoPath
- CreatedDate
- LastLoginDate
- FailedLoginAttempts
- LockedOutUntil
- LastPasswordChangedDate
- PasswordHistory

### AuditLogs Table
- Id (PK)
- MemberId
- Action
- IpAddress
- UserAgent
- Timestamp
- Details

### UserSessions Table
- Id (PK)
- MemberId
- SessionId
- CreatedAt
- ExpiresAt
- IpAddress
- UserAgent
- IsActive

## Security Features Checklist

- [x] Password complexity requirements (12 chars, upper, lower, numbers, special)
- [x] Password hashing with BCrypt
- [x] NRIC encryption with AES-256
- [x] Session management with timeout
- [x] Account lockout after 3 failed attempts
- [x] CAPTCHA integration
- [x] SQL injection prevention
- [x] XSS prevention
- [x] CSRF protection
- [x] Input validation (client & server)
- [x] Audit logging
- [x] Secure file uploads
- [x] Security headers
- [x] Password history (last 2 passwords)
- [x] Password age policies
- [x] Session hijacking prevention
- [x] Multiple device login detection

## Testing Recommendations

### Use GitHub Codespace / GitHub Advanced Security
1. Push code to GitHub repository
2. Enable GitHub Advanced Security
3. Run security scanning
4. Review and fix any vulnerabilities

### Manual Testing Checklist
- [ ] Register with valid data
- [ ] Register with duplicate email (should fail)
- [ ] Register with weak password (should fail)
- [ ] Register without CAPTCHA (should fail)
- [ ] Login with valid credentials
- [ ] Login with invalid credentials (3 times to trigger lockout)
- [ ] Wait 15 minutes and login again
- [ ] Change password
- [ ] Try to reuse old password (should fail)
- [ ] Try to change password within 5 minutes (should fail)
- [ ] Upload resume (.pdf and .docx)
- [ ] Upload photo (optional)
- [ ] Test session timeout (wait 30 minutes)
- [ ] Test logout functionality
- [ ] View profile page
- [ ] Test XSS in "Who Am I" field
- [ ] Test SQL injection in login/register forms

## Notes for Production

1. **HTTPS Only:** Ensure application runs on HTTPS in production
2. **ReCAPTCHA:** Must configure valid reCAPTCHA keys
3. **Database:** Use proper SQL Server instance, not LocalDB
4. **Secrets:** Move sensitive keys to environment variables or Azure Key Vault
5. **Logging:** Implement proper logging (Application Insights, Serilog)
6. **Error Messages:** Remove detailed error messages in production
7. **CORS:** Update CORS policy for your frontend domain
8. **File Storage:** Consider cloud storage (Azure Blob) for uploaded files

## Troubleshooting

### Database Issues
```bash
# Drop database and recreate
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Build Issues
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Port Issues
- Check `Properties/launchSettings.json` for port configuration
- Update frontend URLs if ports change

## Future Enhancements (Optional)

- Two-Factor Authentication (2FA)
- Email verification
- Password reset via email/SMS
- Role-based access control
- API rate limiting
- Redis for distributed sessions
- Docker containerization
- CI/CD pipeline

## License

This project is for educational purposes as part of Application Security coursework.

## Author

[Your Name]
[Module Group]
[Date]
