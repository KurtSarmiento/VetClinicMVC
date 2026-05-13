using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogActionAsync(string userId, string username, string action, string entityName,
            int? entityId, string description, string? oldValues = null, string? newValues = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Username = username,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepository.LogActionAsync(log);
        }

        public async Task<IEnumerable<AuditLog>> GetRecentLogsAsync(int count = 50)
        {
            return await _auditLogRepository.GetRecentLogsAsync(count);
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, int? entityId = null)
        {
            return await _auditLogRepository.GetLogsByEntityAsync(entityName, entityId);
        }
    }
}