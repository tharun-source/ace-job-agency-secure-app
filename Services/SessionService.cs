using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Application_Security_Asgnt_wk12.Services
{
    public class SessionService
    {
     private readonly ApplicationDbContext _context;
  private const int SessionTimeoutMinutes = 30;  // Changed from 1 to 30 minutes

     public SessionService(ApplicationDbContext context)
 {
    _context = context;
}

        public async Task<UserSession> CreateSessionAsync(int memberId, string ipAddress, string userAgent)
 {
          var sessionId = GenerateSecureSessionId();
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

            return session;
        }

     public async Task<UserSession?> GetActiveSessionAsync(string sessionId)
 {
     var session = await _context.UserSessions
     .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.IsActive);

    if (session != null && session.ExpiresAt < DateTime.UtcNow)
  {
       session.IsActive = false;
  await _context.SaveChangesAsync();
      return null;
      }

 return session;
        }

   public async Task<bool> ValidateSessionAsync(string sessionId, string ipAddress, string userAgent)
     {
            var session = await GetActiveSessionAsync(sessionId);
  
            if (session == null)
             return false;

            // Check if IP address and user agent match
 if (session.IpAddress != ipAddress || session.UserAgent != userAgent)
 {
            // Potential session hijacking - invalidate session
   session.IsActive = false;
             await _context.SaveChangesAsync();
      return false;
      }

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
       var sessions = await _context.UserSessions
        .Where(s => s.MemberId == memberId && s.IsActive)
   .ToListAsync();

foreach (var session in sessions)
        {
       session.IsActive = false;
   }

            await _context.SaveChangesAsync();
 }

 public async Task<List<UserSession>> GetActiveUserSessionsAsync(int memberId)
        {
  return await _context.UserSessions
    .Where(s => s.MemberId == memberId && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
  .OrderByDescending(s => s.CreatedAt)
         .ToListAsync();
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
