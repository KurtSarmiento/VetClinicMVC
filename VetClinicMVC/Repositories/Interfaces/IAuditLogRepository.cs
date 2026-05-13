using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task LogActionAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50);
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int? entityId = null);
        Task<IEnumerable<AuditLog>> GetLogsByUserAsync(string userId);
    }
}