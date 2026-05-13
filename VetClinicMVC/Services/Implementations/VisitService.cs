using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class VisitService : IVisitService
    {
        private readonly IVisitRepository _visitRepository;
        private readonly IAuditLogService _auditLogService;

        public VisitService(IVisitRepository visitRepository, IAuditLogService auditLogService)
        {
            _visitRepository = visitRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Visit?> GetByIdAsync(int id)
        {
            return await _visitRepository.GetVisitWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByPetIdAsync(int petId)
        {
            return await _visitRepository.GetVisitsByPetIdAsync(petId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByStaffIdAsync(int staffId)
        {
            return await _visitRepository.GetVisitsByStaffIdAsync(staffId);
        }

        public async Task<IEnumerable<Visit>> GetRecentVisitsAsync(int count = 10)
        {
            return await _visitRepository.GetRecentVisitsAsync(count);
        }

        public async Task CreateVisitAsync(Visit visit, string currentUserId)
        {
            await _visitRepository.AddAsync(visit);
            await _visitRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Visit",
                visit.VisitId, $"New visit recorded for pet {visit.PetId}");
        }

        public async Task UpdateVisitAsync(Visit visit, string currentUserId)
        {
            _visitRepository.Update(visit);
            await _visitRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Visit",
                visit.VisitId, $"Visit updated for pet {visit.PetId}");
        }

        public async Task<IEnumerable<Visit>> GetVisitsByClientIdAsync(int clientId)
        {
            return await _visitRepository.GetVisitsByClientIdAsync(clientId);

        }
    }
}