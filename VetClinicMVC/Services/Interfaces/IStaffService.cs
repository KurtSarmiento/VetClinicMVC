using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IStaffService
    {
        Task<Staff?> GetByIdAsync(int id);
        Task<IEnumerable<Staff>> GetAllActiveAsync();
        Task AddStaffAsync(Staff staff, string currentUserId);
        Task UpdateStaffAsync(Staff staff, string currentUserId);
        Task<IEnumerable<Staff>> GetVeterinariansAsync();
    }
}