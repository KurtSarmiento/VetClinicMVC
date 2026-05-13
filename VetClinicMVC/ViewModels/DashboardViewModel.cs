using VetClinicMVC.Models;

namespace VetClinicMVC.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public int TotalClients { get; set; }
        public int TotalPets { get; set; }
        public int TotalAppointmentsToday { get; set; }
        public IEnumerable<Visit> RecentVisits { get; set; } = new List<Visit>();
        public decimal TodayRevenue { get; set; }
    }
}