using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(VetClinicContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int count = 10)
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .Where(a => a.AppointmentDate >= DateTime.UtcNow && a.Status != "Cancelled")
                .OrderBy(a => a.AppointmentDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForStaffAsync(int staffId, DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .Where(a => a.StaffId == staffId);

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            return await query.OrderBy(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClientIdAsync(int clientId)
        {
            return await _context.Appointments
                .Include(a => a.Pet)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
    }
}