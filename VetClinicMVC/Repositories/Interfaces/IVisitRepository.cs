using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IVisitRepository : IGenericRepository<Visit>
    {
        Task<Visit?> GetVisitWithDetailsAsync(int visitId);
        Task<IEnumerable<Visit>> GetVisitsByPetIdAsync(int petId);
        Task<IEnumerable<Visit>> GetVisitsByStaffIdAsync(int staffId);
        Task<IEnumerable<Visit>> GetRecentVisitsAsync(int count = 10);
    }
}