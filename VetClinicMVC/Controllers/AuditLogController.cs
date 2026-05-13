using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var logs = await _auditLogService.GetRecentLogsAsync(100);
            return View(logs);
        }
    }
}