using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        Task<IEnumerable<Pet>> GetPetsByClientIdAsync(int clientId);
        Task<Pet?> GetPetWithDetailsAsync(int petId);
        Task<IEnumerable<Pet>> GetAllPetsWithClientsAsync();
        Task<IEnumerable<Species>> GetAllSpeciesAsync();
        Task<IEnumerable<Breed>> GetBreedsBySpeciesAsync(int speciesId);
    }
}