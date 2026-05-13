using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IVisitService
    {
        Task<Visit?> GetByIdAsync(int id);
        Task<IEnumerable<Visit>> GetVisitsByPetIdAsync(int petId);
        Task<IEnumerable<Visit>> GetVisitsByStaffIdAsync(int staffId);
        Task<IEnumerable<Visit>> GetRecentVisitsAsync(int count = 10);

        Task CreateVisitAsync(Visit visit, string currentUserId);
        Task UpdateVisitAsync(Visit visit, string currentUserId);
    }
}