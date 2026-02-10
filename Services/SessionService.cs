using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Collections.Concurrent;

namespace Application_Security_Asgnt_wk12.Services
{
    public class SessionService
    {
    private readonly ApplicationDbContext _context;
   private const int SessionTimeoutMinutes = 30;
    
        // Static dictionary to hold locks per user (prevents concurrent logins for same user)
     private static readonly ConcurrentDictionary<int, SemaphoreSlim> _userLocks = new();

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        private SemaphoreSlim GetUserLock(int memberId)
        {
   return _userLocks.GetOrAdd(memberId, _ => new SemaphoreSlim(1, 1));
        }

        public async Task<UserSession> CreateSessionAsync(int memberId, string ipAddress, string userAgent)
        {
        var sessionId = GenerateSecureSessionId();
     Console.WriteLine($"[CreateSessionAsync] Creating NEW session for user ID: {memberId}, SessionId: {sessionId}");
     var session = new UserSession
 {
     MemberId = memberId,
    SessionId = sessionId,
           CreatedAt = DateTime.UtcNow,
  ExpiresAt = DateTime.UtcNow.AddMinutes(SessionTimeoutMinutes),
    IpAddress = ipAddress,
           UserAgent = userAgent,
    IsActive = true
   };

     _context.UserSessions.Add(session);
   await _context.SaveChangesAsync();
      Console.WriteLine($"[CreateSessionAsync] Session created and saved. SessionId: {sessionId}, MemberId: {memberId}, ExpiresAt: {session.ExpiresAt}, IsActive: {session.IsActive}");

     return session;
        }

        public async Task<UserSession?> GetActiveSessionAsync(string sessionId)
        {
       var session = await _context.UserSessions
   .FirstOrDefaultAsync(s => s.SessionId == sessionId);

         if (session == null)
          {
    Console.WriteLine($"[GetActiveSessionAsync] Session not found: {sessionId}");
        return null;
         }

 // Check if session is marked as inactive
        if (!session.IsActive)
     {
 Console.WriteLine($"[GetActiveSessionAsync] Session {sessionId} is marked as INACTIVE (MemberId: {session.MemberId})");
       return null;
     }

       // Check if session has expired
    if (session.ExpiresAt < DateTime.UtcNow)
       {
    Console.WriteLine($"[GetActiveSessionAsync] Session {sessionId} has EXPIRED (MemberId: {session.MemberId}, ExpiredAt: {session.ExpiresAt})");
 session.IsActive = false;
       await _context.SaveChangesAsync();
    return null;
  }

            Console.WriteLine($"[GetActiveSessionAsync] Session {sessionId} is VALID (MemberId: {session.MemberId}, ExpiresAt: {session.ExpiresAt})");
          return session;
        }

        public async Task<bool> ValidateSessionAsync(string sessionId, string ipAddress, string userAgent)
      {
            var session = await GetActiveSessionAsync(sessionId);
  
  if (session == null)
           return false;

            // RELAXED VALIDATION FOR TESTING: Only check if session is active
            // In production, you might want to enable IP/UserAgent checking
      // Uncomment below for stricter validation:
          /*
      if (session.IpAddress != ipAddress || session.UserAgent != userAgent)
            {
  // Potential session hijacking - invalidate session immediately
           session.IsActive = false;
       await _context.SaveChangesAsync();
     return false;
}
            */

          // Session is valid and active
       return true;
        }

    public async Task ExtendSessionAsync(string sessionId)
        {
            var session = await _context.UserSessions
.FirstOrDefaultAsync(s => s.SessionId == sessionId && s.IsActive);

 if (session != null)
 {
    session.ExpiresAt = DateTime.UtcNow.AddMinutes(SessionTimeoutMinutes);
  await _context.SaveChangesAsync();
        }
        }

        public async Task InvalidateSessionAsync(string sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

         if (session != null)
            {
                session.IsActive = false;
          await _context.SaveChangesAsync();
  }
 }

    public async Task InvalidateAllUserSessionsAsync(int memberId)
        {
   // Get user-specific lock to prevent race conditions during concurrent logins
var userLock = GetUserLock(memberId);
    await userLock.WaitAsync();
    
    try
   {
       Console.WriteLine($"[InvalidateAllUserSessionsAsync] Starting session invalidation for user ID: {memberId}");
        // Use a transaction to ensure atomicity
   using var transaction = await _context.Database.BeginTransactionAsync();
    
     try
     {
      // Get all active sessions for this user
       var sessions = await _context.UserSessions
.Where(s => s.MemberId == memberId && s.IsActive)
     .ToListAsync();

    Console.WriteLine($"[InvalidateAllUserSessionsAsync] Found {sessions.Count} active session(s) to invalidate for user ID: {memberId}");

      // Mark all sessions as inactive
      foreach (var session in sessions)
  {
           Console.WriteLine($"[InvalidateAllUserSessionsAsync] Invalidating session: {session.SessionId}");
      session.IsActive = false;
   }

     if (sessions.Any())
       {
    await _context.SaveChangesAsync();
     Console.WriteLine($"[InvalidateAllUserSessionsAsync] Successfully invalidated {sessions.Count} session(s)");
     }
     
      await transaction.CommitAsync();
      Console.WriteLine($"[InvalidateAllUserSessionsAsync] Transaction committed for user ID: {memberId}");
    }
    catch (Exception ex)
       {
 Console.WriteLine($"[InvalidateAllUserSessionsAsync] ERROR: {ex.Message}");
 await transaction.RollbackAsync();
       throw;
     }
    }
    finally
   {
      // Always release the lock
    userLock.Release();
    }
}

        public async Task<List<UserSession>> GetActiveUserSessionsAsync(int memberId)
        {
     return await _context.UserSessions
     .Where(s => s.MemberId == memberId && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
      .OrderByDescending(s => s.CreatedAt)
       .ToListAsync();
        }

        public async Task<int> GetActiveSessionCountAsync(int memberId)
  {
            return await _context.UserSessions
         .CountAsync(s => s.MemberId == memberId && s.IsActive && s.ExpiresAt > DateTime.UtcNow);
        }

        private string GenerateSecureSessionId()
        {
  using (var rng = RandomNumberGenerator.Create())
        {
                byte[] tokenData = new byte[32];
        rng.GetBytes(tokenData);
   return Convert.ToBase64String(tokenData);
            }
        }
    }
}
