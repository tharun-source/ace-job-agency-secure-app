# ?? PASSWORD AGE POLICY - QUICK TEST GUIDE

## ?? How to Test

### **Test 1: View Password Status on Profile**

1. **Login** to your account
2. **Go to profile page**
3. **Check for warnings:**
   - If password < 80 days old: No warning
   - If password 80-89 days old: Yellow warning banner
   - If recently changed (< 24 hours): Blue info banner

---

### **Test 2: Minimum Age Enforcement**

1. **Change your password** via Change Password page
2. **Immediately try to change it again**
3. **Expected Result:**
   ```
 ? Error: "You must wait at least 1 day(s) before changing your password again. 
   Time remaining: 24 hours."
   ```
4. **Go back to profile page**
5. **Expected:** Blue info banner showing hours until can change

---

### **Test 3: Expiration Warning** (Simulated)

**Option A: Quick Test (Temporary Change)**

Edit `PasswordService.cs` temporarily:
```csharp
// Change for testing only
public const int MaximumPasswordAgeDays = 1; // Instead of 90
public const int PasswordExpirationWarningDays = 1; // Instead of 10
```

1. Rebuild application
2. Login and go to profile
3. Wait 1 hour
4. Refresh profile page
5. **Expected:** Yellow warning banner appears

**Option B: Database Simulation**

```sql
-- Set password to 85 days old
UPDATE Members 
SET LastPasswordChangedDate = DATEADD(DAY, -85, GETUTCDATE())
WHERE Email = 'your-test@email.com';
```

1. Login and go to profile
2. **Expected:** Warning banner showing "5 days until expiration"

---

### **Test 4: Password Expiration (Login Block)**

```sql
-- Set password to 91 days old (expired)
UPDATE Members 
SET LastPasswordChangedDate = DATEADD(DAY, -91, GETUTCDATE())
WHERE Email = 'your-test@email.com';
```

1. **Logout** (if logged in)
2. **Try to login**
3. **Expected Result:**
```
   ? Error: "Your password has expired. Please reset your password using 'Forgot Password'."
   ```
4. **Click "Forgot Password"**
5. **Reset your password**
6. **Login should work now**

---

## ?? Expected Behaviors

### **Profile Page Warnings**

| Days Since Change | Warning Type | Message |
|-------------------|--------------|---------|
| 0-79 days | None | No warning shown |
| 80-89 days | ?? Yellow Warning | "Password will expire in X days" |
| < 24 hours | ?? Blue Info | "Can change again in X hours" |
| 90+ days | ?? Login Blocked | Cannot login, must reset |

### **Change Password Responses**

| Scenario | Response |
|----------|----------|
| Changed < 1 day ago | ? "Must wait 1 day(s), X hours remaining" |
| Changed ? 1 day ago | ? Allowed to change |
| Same as current password | ? "New password must be different" |
| In password history | ? "Cannot reuse last 2 passwords" |

---

## ?? Quick Visual Test

### **Step 1: Fresh Login**
```
????????????????? PROFILE PAGE ?????????????????????
? Welcome to Your Profile         ?
?         ?
? [No password warnings]    ?
?       ?
? Personal Information              ?
? Name: John Doe    ?
? Email: john@example.com   ?
? Last Password Change: 30 days ago           ?
????????????????????????????????????????????????????
```

### **Step 2: After Changing Password**
```
????????????????? PROFILE PAGE ?????????????????????
? Welcome to Your Profile  ?
?               ?
? ?? You recently changed your password. You can  ?
? change it again in 24 hour(s).      ?
?         ?
? Personal Information              ?
? Name: John Doe         ?
? Last Password Change: Just now      ?
????????????????????????????????????????????????????
```

### **Step 3: 85 Days Later (Simulated)**
```
????????????????? PROFILE PAGE ?????????????????????
? Welcome to Your Profile      ?
?        ?
? ?? Password Warning: Your password will expire   ?
? in 5 days. Please change it soon to avoid being ?
? locked out. [Change Password Now]  ?
?           ?
? Personal Information         ?
? Name: John Doe    ?
? Last Password Change: 85 days ago           ?
????????????????????????????????????????????????????
```

### **Step 4: 91 Days Later (Login Blocked)**
```
????????????????? LOGIN PAGE ???????????????????????
? ? Your password has expired. Please reset your ?
? password using 'Forgot Password'.            ?
?   ?
? Days expired: 1  ?
?   ?
? [Forgot Password]      ?
????????????????????????????????????????????????????
```

---

## ?? Detailed Test Steps

### **Complete Test Procedure (15 minutes)**

#### **Part 1: Normal Operation (2 min)**
1. Login ?
2. View profile - no warnings ?
3. Navigate around app ?

#### **Part 2: Change Password (3 min)**
1. Click "Change Password" ?
2. Enter current password ?
3. Enter new strong password ?
4. Submit form ?
5. See success message ?

#### **Part 3: Minimum Age Test (2 min)**
1. Immediately go to "Change Password" again ?
2. Try to change password ?
3. See error: "Must wait 1 day" ?
4. Go to profile page ?
5. See blue info banner ?

#### **Part 4: Database Simulation (5 min)**
1. Open database ?
2. Run SQL to set password age to 85 days ?
3. Refresh profile page ?
4. See yellow warning banner ?
5. Click "Change Password Now" link ?

#### **Part 5: Expiration Test (3 min)**
1. Run SQL to set password age to 91 days ?
2. Logout ?
3. Try to login ?
4. See expiration error ?
5. Use "Forgot Password" to reset ?

---

## ?? Test Checklist

### **Minimum Age**
- [ ] Cannot change password immediately after changing
- [ ] Error message shows hours remaining
- [ ] Profile shows blue info banner
- [ ] Can change after 24 hours

### **Maximum Age Warning**
- [ ] Warning shows when 10 days left
- [ ] Warning message is clear
- [ ] "Change Password Now" link works
- [ ] Warning accurate (shows correct days)

### **Maximum Age Expiration**
- [ ] Login blocked when password expires (90 days)
- [ ] Error message clear
- [ ] "Forgot Password" link provided
- [ ] Can reset and login again

### **UI Display**
- [ ] Warnings styled correctly (yellow/blue)
- [ ] Messages clear and helpful
- [ ] Links functional
- [ ] No console errors

---

## ?? Troubleshooting

### **Issue: No warning banner showing**

**Possible Causes:**
1. Password not old enough (< 80 days)
2. JavaScript error - check console (F12)
3. API response not including passwordStatus

**Solution:**
```javascript
// Check in browser console (F12)
console.log(data.passwordStatus);
```

### **Issue: Can change password immediately**

**Possible Causes:**
1. Database not updating LastPasswordChangedDate
2. Minimum age check not working

**Solution:**
```sql
-- Check last change date
SELECT LastPasswordChangedDate FROM Members WHERE Id = 1;
```

### **Issue: Not blocked at login when expired**

**Possible Causes:**
1. Check not running in AuthController
2. Password not actually expired

**Solution:**
- Check server console logs
- Verify LastPasswordChangedDate in database

---

## ? Success Criteria

Your implementation is working if:

- [ ] Profile shows appropriate warnings
- [ ] Cannot change password within 24 hours
- [ ] Login blocked after 90 days
- [ ] Warning shown at 80 days (10 days before expiration)
- [ ] UI clear and user-friendly
- [ ] All error messages helpful
- [ ] Audit logs recording events

---

## ?? For Assignment Demo

### **Screenshots to Take:**

1. **Profile page - No warning** (fresh password)
2. **Profile page - Blue info banner** (just changed password)
3. **Profile page - Yellow warning** (80-89 days old)
4. **Change password error** (minimum age violation)
5. **Login blocked** (expired password)
6. **Audit logs** showing password age events

### **Live Demo Script:**

```
1. "Here's the profile page showing password status..."
2. "Watch what happens when I try to change password immediately..."
3. "Notice the error about waiting 24 hours..."
4. "Now I'll simulate an old password..." (run SQL)
5. "See the warning banner appear..."
6. "And if I set it to 91 days..." (run SQL)
7. "The system blocks login and requires a reset..."
```

---

**Ready to Test!** ??

**Estimated Time:** 15 minutes for complete testing  
**Required:** Database access, browser, running application  
**Status:** ? READY
