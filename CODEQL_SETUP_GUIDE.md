# ?? GitHub Code Scanning Setup Guide

## Overview
GitHub Code Scanning (CodeQL) automatically analyzes your code to find security vulnerabilities and coding errors.

---

## ?? Quick Setup (3 Steps)

### Step 1: Enable Code Scanning on GitHub

1. **Navigate to your repository:**
   - https://github.com/tharun-source/ace-job-agency-secure-app

2. **Go to Security tab:**
   - Click **"Security"** at the top of your repo

3. **Set up code scanning:**
   - Click **"Set up code scanning"** button
   - Choose **"CodeQL Analysis"**
   - Click **"Set up this workflow"**

---

### Step 2: Create CodeQL Workflow File

GitHub will create `.github/workflows/codeql.yml` with this content:

```yaml
name: "CodeQL Security Analysis"

on:
  push:
    branches: [ "main", "master" ]
  pull_request:
    branches: [ "main", "master" ]
  schedule:
    - cron: '0 6 * * 1'  # Every Monday at 6 AM

jobs:
  analyze:
    name: Analyze C# Code
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout repository
  uses: actions/checkout@v4

    - name: Setup .NET 8
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'

    - name: Initialize CodeQL
      uses: github/codeql-action/init@v3
    with:
        languages: csharp
  queries: security-extended,security-and-quality

    - name: Build project
    run: |
        dotnet restore "Application Security Asgnt wk12/Application Security Asgnt wk12.csproj"
        dotnet build "Application Security Asgnt wk12/Application Security Asgnt wk12.csproj" --configuration Release

    - name: Perform CodeQL Analysis
      uses: github/codeql-action/analyze@v3
```

---

### Step 3: Commit and Push

1. **Commit the workflow file:**
   ```bash
 git add .github/workflows/codeql.yml
   git commit -m "Add CodeQL security scanning"
   git push origin main
   ```

2. **First scan will run automatically!**

---

## ?? What CodeQL Scans For

### Security Vulnerabilities

| Category | Examples |
|----------|----------|
| **SQL Injection** | Unparameterized queries |
| **XSS (Cross-Site Scripting)** | Unencoded user input |
| **Path Traversal** | Unsafe file operations |
| **Command Injection** | Unvalidated shell commands |
| **Hardcoded Secrets** | API keys, passwords in code |
| **Insecure Deserialization** | Untrusted data deserialization |
| **LDAP Injection** | Unvalidated LDAP queries |
| **XML External Entities** | XXE vulnerabilities |
| **Unvalidated Redirects** | Open redirect vulnerabilities |
| **Sensitive Data Exposure** | Logging sensitive information |

### Code Quality Issues

- Null reference exceptions
- Resource leaks (unclosed connections)
- Logic errors
- Unused code
- Performance issues
- Best practice violations

---

## ?? Query Suites

We've configured **two query suites** for maximum coverage:

### 1. **security-extended**
- All security vulnerabilities
- High confidence detections
- Minimal false positives

### 2. **security-and-quality**
- Security issues
- Code quality problems
- Best practice violations
- Performance issues

---

## ?? Viewing Scan Results

### After First Scan

1. **Go to Security tab** in your repo
2. **Click "Code scanning"** in left sidebar
3. You'll see:
   ```
   ? Latest scan: [Date/Time]
   ? Total alerts: X
   ? Critical: 0
   ? High: 0
   ? Medium: X
   ? Low: X
   ```

### Understanding Alert Levels

| Level | Severity | Action Required |
|-------|----------|----------------|
| **Critical** | ?? Exploit ready | Fix immediately |
| **High** | ?? Security risk | Fix ASAP |
| **Medium** | ?? Potential issue | Fix soon |
| **Low** | ?? Best practice | Consider fixing |

---

## ?? When Scans Run

CodeQL will automatically scan your code:

### 1. **On Every Push** to main/master branch
```yaml
on:
  push:
    branches: [ "main", "master" ]
```

### 2. **On Every Pull Request**
```yaml
  pull_request:
branches: [ "main", "master" ]
```

### 3. **Weekly Schedule** (Every Monday)
```yaml
  schedule:
    - cron: '0 6 * * 1'
```

---

## ??? Manual Scan Trigger

You can also trigger scans manually:

1. Go to **Actions** tab
2. Click **"CodeQL Security Analysis"** workflow
3. Click **"Run workflow"** button
4. Select branch and click **"Run workflow"**

---

## ?? Workflow Status

### Check Scan Status

1. **Actions tab** ? Shows all workflow runs
2. **Green ?** = Scan completed successfully
3. **Red ?** = Scan failed (check logs)
4. **Yellow ?** = Scan in progress

### View Detailed Logs

- Click on any workflow run
- Expand each step to see detailed logs
- Useful for troubleshooting build issues

---

## ?? Troubleshooting

### Common Issues

#### 1. **Build Fails**

**Error:** `Project not found`

**Solution:** Update the project path in workflow:
```yaml
dotnet restore "YOUR_PROJECT_PATH/YOUR_PROJECT.csproj"
```

#### 2. **Missing Dependencies**

**Error:** `Package not found`

**Solution:** Ensure all NuGet packages are in your `.csproj`:
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

#### 3. **Timeout**

**Error:** `Workflow timed out`

**Solution:** Increase timeout:
```yaml
timeout-minutes: 360  # 6 hours
```

---

## ?? Expected Results for Your Project

Based on your current code, you should see:

### ? **Expected: 0 Critical Issues**
- No SQL injection (using EF Core parameterized queries)
- No XSS (HTML encoding implemented)
- No hardcoded secrets (using appsettings.json)

### ? **Expected: 0 High Issues**
- Secure password hashing (BCrypt)
- Input validation implemented
- CSRF protection enabled

### ?? **Possible: Few Low/Medium Issues**
- Code style suggestions
- Null reference warnings
- Performance optimizations

---

## ?? Optimizing Your Score

### To Get 0 Alerts:

1. **Fix all Critical/High alerts** immediately
2. **Review Medium alerts** and fix security-related ones
3. **Consider Low alerts** for code quality
4. **Suppress false positives** with comments:
```csharp
// lgtm [csharp/sql-injection]
// This is safe because input is validated
```

---

## ?? Screenshot Checklist

For your assignment report, capture:

1. ? **Security tab** showing "Code scanning alerts • Enabled"
2. ? **CodeQL workflow** in Actions tab (green ?)
3. ? **Scan results** showing "0 critical issues"
4. ? **Alert history** graph (if any alerts were fixed)
5. ? **Workflow file** in `.github/workflows/codeql.yml`

---

## ?? Assignment Report Section

### What to Include:

```markdown
## GitHub Code Scanning (CodeQL)

**Status:** ? Enabled

**Configuration:**
- **Language:** C# (.NET 8)
- **Query Suites:** security-extended, security-and-quality
- **Scan Frequency:** On push, pull requests, and weekly

**Results:**
- Total Scans: X
- Critical Issues: 0 ?
- High Issues: 0 ?
- Medium Issues: X
- Low Issues: X

**Screenshot:** [Insert CodeQL results screenshot]

**Conclusion:** All critical and high-severity vulnerabilities have been addressed. The application has been verified through automated security analysis.
```

---

## ?? Security Badge (Optional)

Add a badge to your README.md:

```markdown
[![CodeQL](https://github.com/tharun-source/ace-job-agency-secure-app/workflows/CodeQL/badge.svg)](https://github.com/tharun-source/ace-job-agency-secure-app/actions?query=workflow%3ACodeQL)
```

This shows:
- ? Green badge if scan passes
- ? Red badge if issues found

---

## ?? Additional Resources

- **CodeQL Documentation:** https://codeql.github.com/docs/
- **CodeQL for C#:** https://codeql.github.com/docs/codeql-language-guides/codeql-for-csharp/
- **GitHub Security:** https://docs.github.com/en/code-security

---

## ? Verification Checklist

Before submitting your assignment:

- [ ] CodeQL workflow file created (`.github/workflows/codeql.yml`)
- [ ] Workflow file committed and pushed to GitHub
- [ ] First scan completed successfully
- [ ] Security tab shows "Code scanning alerts • Enabled"
- [ ] All critical and high alerts addressed (if any)
- [ ] Screenshots captured for report
- [ ] CodeQL section added to assignment report
- [ ] Badge added to README (optional)

---

## ?? Summary

**What You've Accomplished:**

1. ? **Automated Security Scanning** - Runs on every code change
2. ? **Continuous Monitoring** - Weekly scans catch new vulnerabilities
3. ? **Industry Standard** - Using GitHub's official CodeQL engine
4. ? **Comprehensive Coverage** - Detects 100+ types of vulnerabilities
5. ? **Assignment Compliance** - Meets "Source Code Analysis (5%)" requirement

**Next Steps:**

1. Commit and push the workflow file
2. Wait for first scan to complete (~5-10 minutes)
3. Review results in Security tab
4. Take screenshots for your report
5. Document findings in assignment report

---

**Status:** ?? Ready to Implement  
**Difficulty:** ? Easy (GitHub handles everything)  
**Time Required:** ?? 5 minutes setup + 10 minutes first scan  
**Marks:** 5% (Source Code Analysis requirement)

---

**Last Updated:** ${new Date().toISOString()}  
**Created For:** Application Security Assignment  
**Project:** Ace Job Agency Secure Web Application
