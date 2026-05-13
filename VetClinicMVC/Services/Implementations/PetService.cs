using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IAuditLogService _auditLogService;

        public PetService(IPetRepository petRepository, IAuditLogService auditLogService)
        {
            _petRepository = petRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _petRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetPetsByClientIdAsync(int clientId)
        {
            return await _petRepository.GetPetsByClientIdAsync(clientId);
        }

        public async Task AddPetAsync(Pet pet, string currentUserId)
        {
            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Pet",
                pet.PetId, $"New pet added: {pet.Name}");
        }

        public async Task UpdatePetAsync(Pet pet, string currentUserId)
        {
            _petRepository.Update(pet);
            await _petRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Pet",
                pet.PetId, $"Pet updated: {pet.Name}");
        }

        public async Task DeletePetAsync(int id, string currentUserId)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet != null)
            {
                _petRepository.Remove(pet);
                await _petRepository.SaveChangesAsync();

                await _auditLogService.LogActionAsync(currentUserId, "System", "Delete", "Pet",
                    id, $"Pet deleted: {pet.Name}");
            }
        }
    }
}