# ? QUICK START - Push to GitHub in 5 Commands

## ?? **FASTEST WAY TO PUSH YOUR PROJECT**

Open PowerShell in your project folder and run these 5 commands:

---

### **1. Navigate to Project**
```bash
cd "C:\Users\tharu\source\repos\Application Security Asgnt wk12\Application Security Asgnt wk12"
```

### **2. Initialize & Add Files**
```bash
git init
git add .
```

### **3. Commit**
```bash
git commit -m "Initial commit: Secure Web Application with ASP.NET Core 8"
```

### **4. Connect to GitHub**
```bash
# Replace YOUR-USERNAME with your GitHub username!
git remote add origin https://github.com/YOUR-USERNAME/ace-job-agency-secure-app.git
```

### **5. Push**
```bash
git branch -M main
git push -u origin main
```

---

## ? **DONE!**

Your code is now on GitHub! ??

---

## ??? **NEXT: Enable Security Scanning**

Go to your GitHub repository and enable:

1. **Dependabot alerts**
2. **Code scanning (CodeQL)**
3. **Secret scanning**

**Where?** Repository ? Settings ? Code security and analysis

---

## ?? **TAKE SCREENSHOTS**

For your report, capture:
- ? Repository main page
- ? Security tab (all features enabled)
- ? Dependabot alerts (should be 0)
- ? CodeQL scan results

---

## ?? **TROUBLESHOOTING**

**Authentication Error?**
- Use Personal Access Token as password
- GitHub ? Settings ? Developer settings ? Personal access tokens

**"Not a git repository"?**
- Make sure you're in the correct directory
- Run `git init` first

**Permission Denied?**
- Check your repository URL
- Verify you have write access

---

## ?? **FOR FUTURE UPDATES**

When you make changes:

```bash
git add .
git commit -m "Description of changes"
git push
```

---

## ?? **TIME: 5-10 minutes**

That's it! You're done! ??
