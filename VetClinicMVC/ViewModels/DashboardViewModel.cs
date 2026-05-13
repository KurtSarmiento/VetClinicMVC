namespace VetClinicMVC.ViewModels
{
    public class DashboardViewModel
    {
        // Add these two specific lines:
        public int TotalClients { get; set; }
        public int TotalPets { get; set; }

        public int AppointmentsToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockItems { get; set; }

        public IEnumerable<VetClinicMVC.Models.Appointment> UpcomingAppointments { get; set; } = new List<VetClinicMVC.Models.Appointment>();
        public IEnumerable<VetClinicMVC.Models.Medication> LowStockMedications { get; set; } = new List<VetClinicMVC.Models.Medication>();
        public IEnumerable<VetClinicMVC.Models.Invoice> RecentInvoices { get; set; } = new List<VetClinicMVC.Models.Invoice>();
    }
}