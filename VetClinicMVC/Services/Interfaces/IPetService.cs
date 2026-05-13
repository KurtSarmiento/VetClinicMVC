using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IPetService
    {
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<IEnumerable<Pet>> GetPetsByClientIdAsync(int clientId);
        Task AddPetAsync(Pet pet, string currentUserId);
        Task UpdatePetAsync(Pet pet, string currentUserId);
        Task DeletePetAsync(int id, string currentUserId);
        Task<IEnumerable<Species>> GetAllSpeciesAsync();
        Task<IEnumerable<Breed>> GetBreedsBySpeciesAsync(int speciesId);
    }
}