using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _uow;

        public MedicationService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
        {
            return await _uow.Medications.GetAllAsync();
        }

        public async Task<Medication?> GetByIdAsync(int id)
        {
            return await _uow.Medications.GetByIdAsync(id);
        }

        public async Task<bool> UpdateStockAsync(int medicationId, int quantitySold)
        {
            var med = await _uow.Medications.GetByIdAsync(medicationId);
            if (med == null) return false;

            med.StockQuantity -= quantitySold; // Deduct from inventory

            _uow.Medications.Update(med);
            return await _uow.CompleteAsync() > 0;
        }
        public async Task<bool> AddMedicationAsync(Medication medication)
        {
            await _uow.Medications.AddAsync(medication);
            return await _uow.CompleteAsync() > 0;
        }

        public async Task<IEnumerable<Medication>> GetLowStockAsync()
        {
            // Maximalist: alert if items have less than 5 units left
            return await _uow.Medications.FindAsync(m => m.StockQuantity < 5);
        }
    }
}