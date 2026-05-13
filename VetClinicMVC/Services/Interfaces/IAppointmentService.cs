using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Appointment?> GetByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int count = 10);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end);
        Task CreateAppointmentAsync(Appointment appointment, string currentUserId);
        Task UpdateAppointmentStatusAsync(int appointmentId, string status, string currentUserId);
        Task CancelAppointmentAsync(int appointmentId, string currentUserId, string? reason = null);
    }
}