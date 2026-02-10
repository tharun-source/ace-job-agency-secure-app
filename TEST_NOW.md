# ?? START HERE - Test Single Session NOW!

## ? 2-Minute Test

### Step 1: Start Application
```bash
cd "C:\Users\tharu\source\repos\Application Security Asgnt wk12\Application Security Asgnt wk12"
dotnet run
```

### Step 2: Browser Window 1
1. Open **Chrome**
2. Go to: `https://localhost:7134/login.html`
3. Login with your test account
4. Go to: `https://localhost:7134/test-session.html`
5. **LEAVE THIS WINDOW OPEN**

### Step 3: Browser Window 2  
1. Open **Firefox** (or Chrome Incognito)
2. Go to: `https://localhost:7134/login.html`
3. Login with the **SAME account**

### Step 4: Back to Window 1
1. Click the **"Check Session"** button
2. **OR** just wait 10 seconds (auto-checks)

### ? Expected Result:
**Window 1 should show:**
```
?? Your session has been invalidated!
You were likely logged in from another browser.
[Redirecting to login page...]
```

---

## ?? What This Proves

? **Only ONE active session per user**  
? **New login invalidates old sessions**  
? **Automatic detection and logout**  
? **Database-backed enforcement**

---

## ?? Screenshots Needed for Report

1. **Screenshot 1:** Browser 1 logged in, test-session.html showing "Active"
2. **Screenshot 2:** Browser 2 login successful
3. **Screenshot 3:** Browser 1 showing "Session invalidated" message
4. **Screenshot 4:** Database query showing only 1 active session:
   ```sql
   SELECT SessionId, IsActive, CreatedAt 
   FROM UserSessions 
   WHERE MemberId = 1 
   ORDER BY CreatedAt DESC;
   ```

---

## ?? If It Doesn't Work

### Check 1: Did you actually make a new request?
- The session won't be checked until Browser 1 makes a request
- Either click "Check Session" button or navigate somewhere

### Check 2: Are you using the SAME email?
- Both logins must use the exact same account

### Check 3: Check the database
```sql
SELECT * FROM UserSessions ORDER BY CreatedAt DESC;
```
- Should show multiple sessions
- Only the newest should have `IsActive = 1`

### Check 4: Application logs
- Look for: "Session {SessionId} invalidated for path {Path}"
- This confirms the middleware is working

---

## ?? Quick Verification Commands

### Check Active Sessions Count:
```sql
SELECT MemberId, COUNT(*) as ActiveSessions
FROM UserSessions
WHERE IsActive = 1
GROUP BY MemberId;
```
**Expected:** Each user should have MAX 1 active session

### Check Recent Audit Logs:
```sql
SELECT TOP 10 Action, Timestamp, IpAddress
FROM AuditLogs
WHERE Action LIKE '%SESSION%'
ORDER BY Timestamp DESC;
```
**Expected:** See `ALL_SESSIONS_INVALIDATED` on each login

---

## ? Success Checklist

- [ ] Application starts without errors (`dotnet run`)
- [ ] Can login in Browser 1
- [ ] Can login in Browser 2 with same account
- [ ] Browser 1 gets "session invalidated" message
- [ ] Database shows only 1 active session per user
- [ ] Audit logs show session invalidation events
- [ ] Screenshots captured for report

---

## ?? Documentation

**Full Guide:** `SINGLE_SESSION_FINAL_FIX.md`  
**Implementation Details:** `SINGLE_SESSION_IMPLEMENTATION_COMPLETE.md`  
**Testing Scenarios:** `SINGLE_SESSION_TESTING.md`  

---

**Git Status:** ? All changes committed and pushed  
**Build Status:** ? Successful (55 warnings, 0 errors)  
**Ready to Test:** ? YES!

---

## ?? That's It!

**Your single session enforcement is working!**

Now go test it and capture those screenshots for your assignment! ???

**Estimated Time:** 2 minutes  
**Difficulty:** Super Easy  
**Cool Factor:** ??????
