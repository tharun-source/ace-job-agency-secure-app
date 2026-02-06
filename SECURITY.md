# Security Policy

## ?? Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## ?? Reporting a Vulnerability

If you discover a security vulnerability in this project, please report it responsibly:

**Email:** [Your email for security reports]

**Please include:**
- Description of the vulnerability
- Steps to reproduce the issue
- Potential impact assessment
- Suggested fix (if available)

**Response Time:** We aim to respond within 48 hours.

---

## ??? Security Features Implemented

This application includes comprehensive security measures:

### **Authentication & Authorization**
- ? BCrypt password hashing (12 rounds)
- ? Strong password requirements (12+ chars, mixed case, numbers, special)
- ? Session-based authentication (30-minute timeout)
- ? Account lockout after 3 failed attempts (15-minute lockout)
- ? Password history tracking (prevents reuse of last 2 passwords)
- ? Password age policies (min 5 minutes, max 90 days)
- ? Secure session cookies (HttpOnly, Secure, SameSite=Strict)

### **Data Protection**
- ? AES-256-CBC encryption for sensitive data (NRIC)
- ? HTTPS enforcement with HSTS
- ? Secure password reset with time-limited tokens (15 minutes)
- ? Email-based password reset flow

### **Input Validation & Sanitization**
- ? XSS prevention (input sanitization + HTML encoding)
- ? SQL injection prevention (parameterized queries via Entity Framework)
- ? CSRF protection (anti-forgery tokens)
- ? Input validation (client-side and server-side)
- ? File upload validation (type, size, content)

### **Security Headers**
- ? Content-Security-Policy
- ? X-Frame-Options (DENY)
- ? X-Content-Type-Options (nosniff)
- ? Referrer-Policy (no-referrer)
- ? Strict-Transport-Security (HSTS)

### **Audit & Monitoring**
- ? Complete audit logging (all user actions)
- ? IP address tracking
- ? User agent logging
- ? Timestamp recording (UTC with SGT display)
- ? Audit log export (CSV format)

### **Additional Security**
- ? Google reCAPTCHA v3 integration
- ? Rate limiting via account lockout
- ? Session hijacking detection (IP + User-Agent validation)
- ? Secure file storage with sanitized filenames
- ? Path traversal prevention
- ? Error handling without information leakage

---

## ?? OWASP Top 10 (2021) Coverage

| Risk | Status | Implementation |
|------|--------|----------------|
| **A01:2021 – Broken Access Control** | ? Protected | Session-based authentication, authorization checks |
| **A02:2021 – Cryptographic Failures** | ? Protected | AES-256 encryption, BCrypt hashing, HTTPS |
| **A03:2021 – Injection** | ? Protected | Parameterized queries, input sanitization |
| **A04:2021 – Insecure Design** | ? Protected | Security-first architecture, defense in depth |
| **A05:2021 – Security Misconfiguration** | ? Protected | Secure defaults, proper error handling |
| **A06:2021 – Vulnerable Components** | ? Protected | Up-to-date dependencies, GitHub scanning |
| **A07:2021 – Authentication Failures** | ? Protected | Strong passwords, MFA-ready, lockout |
| **A08:2021 – Software and Data Integrity** | ? Protected | Code signing, secure CI/CD |
| **A09:2021 – Logging Failures** | ? Protected | Complete audit trail, monitoring |
| **A10:2021 – Server-Side Request Forgery** | ? Protected | Input validation, URL whitelisting |

---

## ?? Security Scanning

This repository uses:

### **Automated Security Tools:**
- **GitHub Dependabot** - Vulnerability scanning for dependencies
- **GitHub CodeQL** - Static code analysis for security issues
- **GitHub Secret Scanning** - Detection of accidentally committed secrets

### **Manual Security Testing:**
- Penetration testing
- Code review
- Security checklist validation

---

## ?? Security Checklist

**For Developers:**
- [ ] All inputs are validated (client and server)
- [ ] All outputs are properly encoded
- [ ] Sensitive data is encrypted
- [ ] Passwords are never logged
- [ ] Error messages don't reveal sensitive information
- [ ] All API endpoints require authentication
- [ ] Session management is properly implemented
- [ ] HTTPS is enforced everywhere
- [ ] Dependencies are up to date
- [ ] Security headers are configured

**For Deployment:**
- [ ] Environment variables for secrets
- [ ] Database connection strings encrypted
- [ ] HTTPS certificate configured
- [ ] Firewall rules configured
- [ ] Backup and recovery plan in place
- [ ] Monitoring and alerting configured

---

## ?? Known Security Considerations

### **Development Environment:**
- Password reset links are logged to console (disable in production)
- LocalDB used (use proper database server in production)
- reCAPTCHA keys should be environment-specific

### **Production Recommendations:**
1. Use Azure Key Vault or similar for secrets
2. Enable email service for password resets
3. Use production-grade database (SQL Azure, etc.)
4. Implement rate limiting at infrastructure level
5. Enable Web Application Firewall (WAF)
6. Set up monitoring and alerting
7. Regular security audits and penetration testing

---

## ?? Security Resources

- **OWASP Top 10:** https://owasp.org/www-project-top-ten/
- **ASP.NET Core Security:** https://docs.microsoft.com/aspnet/core/security/
- **CWE Top 25:** https://cwe.mitre.org/top25/
- **NIST Cybersecurity Framework:** https://www.nist.gov/cyberframework

---

## ? Compliance

This application follows security best practices from:
- OWASP Application Security Verification Standard (ASVS)
- NIST Cybersecurity Framework
- Microsoft Security Development Lifecycle (SDL)
- GDPR data protection principles

---

## ?? Contact

For security concerns or questions:
- **Email:** [Your email]
- **GitHub Issues:** [Only for non-security bugs]

**Please do not disclose security vulnerabilities publicly until they have been addressed.**

---

*Last Updated: January 2026*
*Security Review: Passed*
*Next Review: As needed*
