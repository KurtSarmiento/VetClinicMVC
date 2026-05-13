using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        public PetRepository(VetClinicContext context) : base(context) { }

        public async Task<IEnumerable<Pet>> GetPetsByClientIdAsync(int clientId)
        {
            return await _context.Pets
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .Where(p => p.ClientId == clientId && !p.IsDeceased)
                .ToListAsync();
        }

        public async Task<Pet?> GetPetWithDetailsAsync(int petId)
        {
            return await _context.Pets
                .Include(p => p.Client)
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .FirstOrDefaultAsync(p => p.PetId == petId);
        }

        public async Task<IEnumerable<Pet>> GetAllPetsWithClientsAsync()
        {
            return await _context.Pets
                .Include(p => p.Client)
                .Include(p => p.Species)
                .ToListAsync();
        }
    }
}