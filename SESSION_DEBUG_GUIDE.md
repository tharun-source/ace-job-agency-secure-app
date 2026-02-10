# ?? SESSION DEBUG - CHECK CONSOLE LOGS

## ?? What We Did

Added **detailed console logging** to track exactly what's happening with sessions:

1. **AuthController.Login** - Logs when sessions are invalidated and created
2. **SessionService.GetActiveSessionAsync** - Shows why a session is valid or invalid
3. **SessionService.InvalidateAllUserSessionsAsync** - Tracks invalidation process
4. **SessionService.CreateSessionAsync** - Logs session creation

---

## ?? How to Debug

### Step 1: Start the Application

```bash
cd "C:\Users\tharu\source\repos\Application Security Asgnt wk12\Application Security Asgnt wk12"
dotnet run
```

**IMPORTANT:** Keep the console window visible! All debug logs will appear here.

---

### Step 2: Login and Watch Console

1. **Open browser** ? `https://localhost:7134/login.html`
2. **Login** with your test account
3. **Watch the console** - You should see:

```
[CreateSessionAsync] Creating NEW session for user ID: 1, SessionId: ABC123...
[CreateSessionAsync] Session created and saved. SessionId: ABC123..., MemberId: 1, ExpiresAt: 2024-..., IsActive: True
Invalidating all existing sessions for user ID: 1
[InvalidateAllUserSessionsAsync] Starting session invalidation for user ID: 1
[InvalidateAllUserSessionsAsync] Found 0 active session(s) to invalidate for user ID: 1
[InvalidateAllUserSessionsAsync] Transaction committed for user ID: 1
Creating new session for user ID: 1
New session created with ID: ABC123... for user ID: 1
```

---

### Step 3: Navigate to Profile

1. **Click "Profile"** or go to `profile.html`
2. **Watch console** - You should see:

```
[GetActiveSessionAsync] Session ABC123... is VALID (MemberId: 1, ExpiresAt: 2024-...)
```

**This should repeat every time the page makes an API call**

---

### Step 4: Wait 60 Seconds (Auto-Check)

1. **Stay on profile page**
2. **Wait for auto-check** (happens every 60 seconds)
3. **Watch console** - You should see:

```
[GetActiveSessionAsync] Session ABC123... is VALID (MemberId: 1, ExpiresAt: 2024-...)
```

---

## ?? What to Look For

### ? BAD: Session Shows as INACTIVE

If you see this:
```
[GetActiveSessionAsync] Session ABC123... is marked as INACTIVE (MemberId: 1)
```

**This means:**
- The session was created but then immediately marked inactive
- OR something is invalidating the session after creation
- This is the BUG we're looking for!

---

### ? BAD: Session Shows as EXPIRED

If you see this:
```
[GetActiveSessionAsync] Session ABC123... has EXPIRED (MemberId: 1, ExpiredAt: 2024-...)
```

**This means:**
- The ExpiresAt timestamp is in the past
- Could be a timezone issue (UTC vs local time)

---

###? BAD: Session Not Found

If you see this:
```
[GetActiveSessionAsync] Session not found: ABC123...
```

**This means:**
- The session was never saved to the database
- OR the SessionId in the cookie doesn't match database

---

### ? GOOD: Session is VALID

If you see this consistently:
```
[GetActiveSessionAsync] Session ABC123... is VALID (MemberId: 1, ExpiresAt: 2024-...)
```

**This means:**
- Session is working correctly!
- The issue might be elsewhere (frontend, cookies, etc.)

---

## ?? Debug Checklist

After login, check these in order:

### 1. **Session Created Successfully?**
```
? [CreateSessionAsync] Session created and saved
? SessionId shown
? IsActive: True
? ExpiresAt: Future timestamp
```

### 2. **Session Stored in HTTP Session?**
```
? New session created with ID: ...
? No errors shown
```

### 3. **Session Validates on First Check?**
```
? [GetActiveSessionAsync] Session ... is VALID
```

### 4. **Session Validates on Subsequent Checks?**
```
? [GetActiveSessionAsync] Session ... is VALID (repeated)
```

### 5. **Session Extends on Activity?**
```
? ExpiresAt timestamp increases on each check
```

---

## ?? Common Issues & Solutions

### Issue 1: Session Shows as INACTIVE Immediately

**Symptom:**
```
[CreateSessionAsync] IsActive: True
[GetActiveSessionAsync] Session ... is marked as INACTIVE
```

**Cause:** Something is invalidating the session between creation and first check.

**Solution:** Check if there's duplicate code calling `InvalidateAllUserSessionsAsync` after session creation.

---

### Issue 2: ExpiresAt is in the Past

**Symptom:**
```
[CreateSessionAsync] ExpiresAt: 2024-02-10 06:00:00
[GetActiveSessionAsync] Session has EXPIRED (ExpiredAt: 2024-02-10 06:00:00)
Current time: 2024-02-10 15:00:00
```

**Cause:** UTC vs Local Time mismatch.

**Solution:** Ensure all timestamps use `DateTime.UtcNow` consistently.

---

### Issue 3: SessionId Mismatch

**Symptom:**
```
[CreateSessionAsync] SessionId: ABC123...
[GetActiveSessionAsync] Session not found: XYZ789...
```

**Cause:** Cookie not being set or read correctly.

**Solution:** Check `HttpContext.Session.SetString("SessionId", ...)` is working.

---

### Issue 4: Database Not Saving

**Symptom:**
```
[CreateSessionAsync] Session created
[GetActiveSessionAsync] Session not found
```

**Cause:** Transaction not committed or database connection issue.

**Solution:** Check database after login:
```sql
SELECT * FROM UserSessions ORDER BY CreatedAt DESC;
```

---

## ?? Next Steps

### If Logs Show Session is VALID:

The backend is working correctly! The issue is likely:
1. **Frontend JavaScript** - Check browser console (F12)
2. **Cookies** - Session cookie might not be sent
3. **CORS/Credentials** - Check `credentials: 'include'` in fetch calls

### If Logs Show Session is INACTIVE:

There's a bug in the backend:
1. Check for duplicate `InvalidateAllUserSessionsAsync` calls
2. Check for race conditions
3. Check transaction isolation

### If Logs Show Session EXPIRED:

Timezone issue:
1. Verify all code uses `DateTime.UtcNow`
2. Check database timezone settings
3. Verify `ExpiresAt` calculation

---

## ?? Screenshot for Report

Take a screenshot of the console showing:
```
? Session created successfully
? Session validated multiple times
? No "INACTIVE" or "EXPIRED" messages
```

This proves your session management is working correctly!

---

## ? Expected Console Output (Normal Flow)

```
info: Application_Security_Asgnt_wk12.Controllers.AuthController[0]
      Invalidating all existing sessions for user ID: 1
[InvalidateAllUserSessionsAsync] Starting session invalidation for user ID: 1
[InvalidateAllUserSessionsAsync] Found 0 active session(s) to invalidate for user ID: 1
[InvalidateAllUserSessionsAsync] Transaction committed for user ID: 1
info: Application_Security_Asgnt_wk12.Controllers.AuthController[0]
      Creating new session for user ID: 1
[CreateSessionAsync] Creating NEW session for user ID: 1, SessionId: rGHJ3k...
[CreateSessionAsync] Session created and saved. SessionId: rGHJ3k..., MemberId: 1, ExpiresAt: 2024-02-10 16:30:00, IsActive: True
info: Application_Security_Asgnt_wk12.Controllers.AuthController[0]
      New session created with ID: rGHJ3k... for user ID: 1
[GetActiveSessionAsync] Session rGHJ3k... is VALID (MemberId: 1, ExpiresAt: 2024-02-10 16:30:00)
[GetActiveSessionAsync] Session rGHJ3k... is VALID (MemberId: 1, ExpiresAt: 2024-02-10 16:30:00)
[GetActiveSessionAsync] Session rGHJ3k... is VALID (MemberId: 1, ExpiresAt: 2024-02-10 17:00:00)
```

**Note:** ExpiresAt should increase (session extension working)

---

**Ready to test!** Run the app and watch those console logs! ??

---

**Status:** ?? DEBUG MODE ENABLED  
**Action:** Run app and check console output  
**Expected:** Detailed logs showing exactly what's happening
