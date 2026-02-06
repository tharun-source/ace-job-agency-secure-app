using Application_Security_Asgnt_wk12.Data;
using Application_Security_Asgnt_wk12.Models;
using Microsoft.EntityFrameworkCore;

namespace Application_Security_Asgnt_wk12.Services
{
    public class AuditService
    {
      private readonly ApplicationDbContext _context;

  public AuditService(ApplicationDbContext context)
 {
            _context = context;
        }

        public async Task LogActionAsync(string memberId, string action, string ipAddress, string userAgent, string? details = null)
        {
 var auditLog = new AuditLog
    {
                MemberId = memberId,
     Action = action,
   IpAddress = ipAddress,
 UserAgent = userAgent,
   Timestamp = DateTime.UtcNow,
     Details = details ?? ""  // Use empty string if null
     };

   _context.AuditLogs.Add(auditLog);
 await _context.SaveChangesAsync();
        }

     public async Task<List<AuditLog>> GetMemberAuditLogsAsync(string memberId, int take = 50)
     {
        return await _context.AuditLogs
        .Where(a => a.MemberId == memberId)
    .OrderByDescending(a => a.Timestamp)
      .Take(take)
       .ToListAsync();
        }
    }
}
