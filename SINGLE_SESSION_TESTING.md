# Single Session Enforcement Testing Guide

## ? Single Session Feature Implemented

The application now enforces **single session per user** - meaning when a user logs in from a new browser/device, all previous sessions are automatically invalidated.

## ?? How It Works

### On Login (`AuthController.cs` - Line 175-177):
```csharp
// Security Enhancement: Invalidate all other sessions for this user
// This prevents multiple simultaneous logins from different locations
await _sessionService.InvalidateAllUserSessionsAsync(member.Id);
await _auditService.LogActionAsync(member.Id.ToString(), "ALL_SESSIONS_INVALIDATED", ipAddress, userAgent);

// Create new session (only one active at a time)
var session = await _sessionService.CreateSessionAsync(member.Id, ipAddress, userAgent);
```

### Session Validation (`SessionValidationMiddleware.cs`):
- Every request checks if the session is still active in the database
- If session was invalidated (user logged in elsewhere), user is redirected to login
- Session is extended on each valid request

## ?? How to Test Single Session Enforcement

### Test Scenario 1: Same Browser, Different Tabs
1. Open Chrome and login at `https://localhost:7134/login.html`
2. Successfully view profile at `https://localhost:7134/profile.html`
3. Open a NEW INCOGNITO window in Chrome
4. Login with the SAME account
5. Go back to the first tab
6. Try to navigate to any page or refresh
7. **Expected**: First tab should redirect to login (session invalidated)

### Test Scenario 2: Different Browsers
1. Open Chrome and login at `https://localhost:7134/login.html`
2. Successfully view profile at `https://localhost:7134/profile.html`
3. Open Firefox (or Edge)
4. Login with the SAME account at `https://localhost:7134/login.html`
5. Go back to Chrome
6. Try to navigate to any page or refresh
7. **Expected**: Chrome should redirect to login (session invalidated)

### Test Scenario 3: Check Session Count in Database
```sql
-- Check active sessions for a user
SELECT * FROM UserSessions 
WHERE MemberId = 1 AND IsActive = 1
ORDER BY CreatedAt DESC;

-- You should only see ONE active session at a time per user
```

### Test Scenario 4: API Endpoint Test (Optional)
If you want to test via API:

1. Login from Browser 1 - note the sessionId
2. Login from Browser 2 with same account
3. Try using sessionId from Browser 1 in an API call:
```bash
curl -X GET https://localhost:7134/api/member/profile \
  -H "Cookie: SessionId=<old-session-id>" \
  -k
```
**Expected**: Should return 401 Unauthorized

## ?? What You Should See

### ? Expected Behavior:
- ? Only ONE active session per user at any time
- ? Previous sessions immediately invalidated when logging in from new location
- ? Old sessions redirected to login page on next request
- ? Audit logs showing "ALL_SESSIONS_INVALIDATED" events

### ? If Multiple Sessions Are Active:
1. Check that `InvalidateAllUserSessionsAsync` is being called before creating new session
2. Verify database shows `IsActive = 0` for old sessions
3. Check middleware is properly validating sessions on each request
4. Review browser console for errors

## ?? Debugging Tips

### Check Active Sessions:
```sql
SELECT 
    Id,
    MemberId,
    SessionId,
    CreatedAt,
    ExpiresAt,
    IsActive,
    IpAddress,
    UserAgent
FROM UserSessions
WHERE MemberId = <your-member-id>
ORDER BY CreatedAt DESC;
```

### Check Audit Logs:
```sql
SELECT 
    Action,
    Timestamp,
    IpAddress,
  Details
FROM AuditLogs
WHERE MemberId = '<your-member-id>'
    AND Action LIKE '%SESSION%'
ORDER BY Timestamp DESC;
```

## ?? Success Criteria

? Test passes when:
1. User can only have ONE active session at a time
2. Logging in from Browser B logs out Browser A automatically
3. Browser A redirects to login on next request
4. Database shows only one active session per user
5. Audit logs show "ALL_SESSIONS_INVALIDATED" on each new login

## ?? Notes

- Sessions are checked on EVERY request to protected pages
- The redirect happens when the invalidated session tries to make a request
- There might be a small delay (until next page request) before Browser A realizes it's logged out
- This is NORMAL behavior - the logout happens on the server immediately, but Browser A finds out when it makes the next request

## ?? Quick Start Test

**Fastest way to test:**
1. Login in Chrome ? Go to profile page
2. Open Firefox ? Login with same account
3. Go back to Chrome ? **Click any navigation link or refresh**
4. Chrome should redirect to login! ?

---

**Implementation Status:** ? COMPLETE  
**Security Level:** Enhanced - Single session per user enforced  
**Last Updated:** Now
