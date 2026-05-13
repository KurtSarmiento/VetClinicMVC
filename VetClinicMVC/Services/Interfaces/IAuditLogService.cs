using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string userId, string username, string action, string entityName,
                           int? entityId, string description, string? oldValues = null, string? newValues = null);
        Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50);
        Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int? entityId = null);
    }
}