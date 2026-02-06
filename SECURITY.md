# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly.

**Please DO NOT create a public GitHub issue for security vulnerabilities.**

Instead, please email: [your-email@example.com]

Please include:
- Description of the vulnerability
- Steps to reproduce the issue
- Potential impact
- Suggested fix (if any)

We will respond within 48 hours and work with you to address the issue.

## Security Features Implemented

This application implements comprehensive security measures following OWASP Top 10 guidelines:

### Authentication & Authorization
- ? Secure session management (30-minute timeout)
- ? BCrypt password hashing (12 rounds)
- ? Account lockout after 3 failed login attempts (15-minute lockout)
- ? Rate limiting on login endpoints
- ? Strong password requirements (12+ chars, mixed case, numbers, special chars)
- ? Password history tracking (prevents reuse of last 2 passwords)
- ? Password age policies (min 5 minutes, max 90 days)
- ? Password reset functionality with time-limited tokens (15 minutes)

### Data Protection
- ? AES-256-CBC encryption for sensitive data (NRIC)
- ? HTTPS enforcement
- ? Secure cookie configuration (HttpOnly, Secure, SameSite)
- ? Input validation and sanitization
- ? Output encoding to prevent XSS

### Injection Prevention
- ? SQL injection prevention (Entity Framework parameterized queries)
- ? XSS prevention (input sanitization + HTML encoding)
- ? No direct command execution

### Security Misconfiguration Prevention
- ? Security headers (CSP, X-Frame-Options, X-Content-Type-Options, Referrer-Policy)
- ? HSTS enforcement in production
- ? Error handling without information leakage
- ? Custom error pages

### Logging & Monitoring
- ? Complete audit trail of user activities
- ? IP address and user agent tracking
- ? Session activity monitoring
- ? Failed login attempt tracking
- ? Exportable audit logs (CSV format)

### File Upload Security
- ? File type validation (.pdf, .docx for resumes; .jpg, .jpeg, .png, .gif for photos)
- ? File size limits (5MB for resumes, 2MB for photos)
- ? Filename sanitization
- ? Path traversal prevention
- ? Secure file storage

### Additional Security Measures
- ? CSRF protection (Anti-forgery tokens)
- ? Google reCAPTCHA v3 integration
- ? Session hijacking detection (IP and User-Agent validation)
- ? Automatic session extension on activity
- ? Secure password reset via email with time-limited tokens
- ? Multiple session management

## Security Scanning

This repository uses:
- ? **GitHub Dependabot** - Automated dependency vulnerability scanning
- ? **CodeQL Analysis** - Static application security testing (SAST)
- ? **Secret Scanning** - Detects accidentally committed secrets

## Security Best Practices

When deploying this application:

1. **Environment Variables**: Store sensitive configuration in environment variables, not in code
2. **Database**: Use strong database passwords and restrict network access
3. **HTTPS**: Always use HTTPS in production with valid SSL/TLS certificates
4. **Updates**: Keep all dependencies up to date
5. **Backups**: Regularly backup database and uploaded files
6. **Monitoring**: Monitor audit logs for suspicious activity
7. **Access Control**: Restrict access to production environments

## Compliance

This application follows:
- OWASP Top 10 2021 guidelines
- NIST Cybersecurity Framework principles
- General Data Protection Regulation (GDPR) considerations
- PCI DSS best practices for data protection

## Security Testing

The application has been tested for:
- SQL Injection
- Cross-Site Scripting (XSS)
- Cross-Site Request Forgery (CSRF)
- Authentication bypass
- Session management vulnerabilities
- File upload vulnerabilities
- Information disclosure
- Broken access control

## Acknowledgments

Security is an ongoing process. We appreciate the security community's efforts in making applications more secure.

---

**Last Updated**: January 2026
**Version**: 1.0.0
