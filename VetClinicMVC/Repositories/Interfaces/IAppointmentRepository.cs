using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int count = 10);
        Task<IEnumerable<Appointment>> GetAppointmentsForStaffAsync(int staffId, DateTime? date = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByClientIdAsync(int clientId);
        Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId);
    }
}
