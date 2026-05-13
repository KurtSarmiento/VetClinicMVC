using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAuditLogService _auditLogService;

        public AppointmentService(IAppointmentRepository appointmentRepository, IAuditLogService auditLogService)
        {
            _appointmentRepository = appointmentRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _appointmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int count = 10)
        {
            return await _appointmentRepository.GetUpcomingAppointmentsAsync(count);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _appointmentRepository.GetAppointmentsByDateRangeAsync(start, end);
        }

        public async Task CreateAppointmentAsync(Appointment appointment, string currentUserId)
        {
            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Appointment",
                appointment.AppointmentId,
                $"Appointment created for pet {appointment.PetId} on {appointment.AppointmentDate}");
        }

        public async Task UpdateAppointmentStatusAsync(int appointmentId, string status, string currentUserId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment != null)
            {
                string oldStatus = appointment.Status;
                appointment.Status = status;
                _appointmentRepository.Update(appointment);
                await _appointmentRepository.SaveChangesAsync();

                await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Appointment",
                    appointmentId, $"Appointment status changed from {oldStatus} to {status}");
            }
        }

        public async Task CancelAppointmentAsync(int appointmentId, string currentUserId, string? reason = null)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                if (!string.IsNullOrEmpty(reason))
                    appointment.Notes += $"\nCancelled: {reason}";

                _appointmentRepository.Update(appointment);
                await _appointmentRepository.SaveChangesAsync();

                await _auditLogService.LogActionAsync(currentUserId, "System", "Cancel", "Appointment",
                    appointmentId, $"Appointment cancelled. Reason: {reason}");
            }
        }
    }
}