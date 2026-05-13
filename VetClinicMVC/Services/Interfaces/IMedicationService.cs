using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IMedicationService
    {
        Task<IEnumerable<Medication>> GetAllMedicationsAsync();
        Task<Medication?> GetByIdAsync(int id);
        Task<bool> UpdateStockAsync(int medicationId, int quantitySold);
        Task<IEnumerable<Medication>> GetLowStockAsync();
        Task<bool> AddMedicationAsync(Medication medication);
    }
}