using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(VetClinicContext context) : base(context) { }

        public async Task LogActionAsync(AuditLog auditLog)
        {
            await _dbSet.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int? entityId = null)
        {
            var query = _context.AuditLogs.Where(l => l.EntityName == entityName);

            if (entityId.HasValue)
                query = query.Where(l => l.EntityId == entityId);

            return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByUserAsync(string userId)
        {
            return await _context.AuditLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }
}