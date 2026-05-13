using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class VisitRepository : GenericRepository<Visit>, IVisitRepository
    {
        public VisitRepository(VetClinicContext context) : base(context) { }

        public async Task<Visit?> GetVisitWithDetailsAsync(int visitId)
        {
            return await _context.Visits
                .Include(v => v.Pet)
                    .ThenInclude(p => p.Client)
                .Include(v => v.Staff)
                .FirstOrDefaultAsync(v => v.VisitId == visitId);
        }

        public async Task<IEnumerable<Visit>> GetVisitsByPetIdAsync(int petId)
        {
            return await _context.Visits
                .Include(v => v.Staff)
                .Where(v => v.PetId == petId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetVisitsByStaffIdAsync(int staffId)
        {
            return await _context.Visits
                .Include(v => v.Pet)
                .Where(v => v.StaffId == staffId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visit>> GetRecentVisitsAsync(int count = 10)
        {
            return await _context.Visits
                .Include(v => v.Pet)
                .Include(v => v.Staff)
                .OrderByDescending(v => v.VisitDate)
                .Take(count)
                .ToListAsync();
        }
    }
}