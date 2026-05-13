using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        public StaffRepository(VetClinicContext context) : base(context) { }

        public async Task<IEnumerable<Staff>> GetAllActiveAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Staff>> GetVeterinariansAsync()
        {
            return await _context.Staff
                .Where(s => s.IsActive && s.Role == "Veterinarian")
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<Staff?> GetStaffWithDetailsAsync(int id)
        {
            return await _context.Staff
                .FirstOrDefaultAsync(s => s.StaffId == id);
        }
    }
}