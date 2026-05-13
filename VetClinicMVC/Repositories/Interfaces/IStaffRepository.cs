using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        Task<IEnumerable<Staff>> GetAllActiveAsync();
        Task<IEnumerable<Staff>> GetVeterinariansAsync();
        Task<Staff?> GetStaffWithDetailsAsync(int id);
    }
}