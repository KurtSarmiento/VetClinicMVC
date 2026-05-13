using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.ViewModels;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IInvoiceService _invoiceService;
        private readonly IMedicationService _medicationService;
        private readonly IPetService _petService;
        private readonly IClientService _clientService;

        public DashboardController(
            IAppointmentService appointmentService,
            IInvoiceService invoiceService,
            IMedicationService medicationService,
            IPetService petService,
            IClientService clientService)
        {
            _appointmentService = appointmentService;
            _invoiceService = invoiceService;
            _medicationService = medicationService;
            _petService = petService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            var allInvoices = await _invoiceService.GetAllAsync();
            var lowStock = await _medicationService.GetLowStockAsync();
            var pets = await _petService.GetAllAsync();
            var clients = await _clientService.GetAllActiveAsync();

            var viewModel = new DashboardViewModel
            {
                TotalClients = clients.Count(), // Assigning the value here
                TotalPets = pets.Count(),       // Assigning the value here

                AppointmentsToday = (await _appointmentService.GetUpcomingAppointmentsAsync(100))
                                    .Count(a => a.AppointmentDate.Date == DateTime.Today),
                TotalRevenue = allInvoices.Where(i => i.Status == "Paid").Sum(i => i.TotalAmount),
                LowStockItems = lowStock.Count(),

                UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(5),
                LowStockMedications = lowStock,
                RecentInvoices = allInvoices.OrderByDescending(i => i.IssueDate).Take(5)
            };

            return View(viewModel);
        }
    }
}