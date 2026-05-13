using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;     // You'll need to create this repository too
        private readonly IAuditLogService _auditLogService;

        public StaffService(IStaffRepository staffRepository, IAuditLogService auditLogService)
        {
            _staffRepository = staffRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _staffRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Staff>> GetAllActiveAsync()
        {
            return await _staffRepository.FindAsync(s => s.IsActive);
        }

        public async Task AddStaffAsync(Staff staff, string currentUserId)
        {
            await _staffRepository.AddAsync(staff);
            await _staffRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Staff",
                staff.StaffId, $"New staff added: {staff.FirstName} {staff.LastName} ({staff.Role})");
        }

        public async Task UpdateStaffAsync(Staff staff, string currentUserId)
        {
            _staffRepository.Update(staff);
            await _staffRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Staff",
                staff.StaffId, $"Staff updated: {staff.FirstName} {staff.LastName}");
        }

        public async Task<IEnumerable<Staff>> GetVeterinariansAsync()
        {
            return await _staffRepository.FindAsync(s => s.IsActive && s.Role == "Veterinarian");
        }
    }
}