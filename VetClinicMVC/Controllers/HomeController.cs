using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.ViewModels;

namespace VetClinicMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IClientService _clientService;
        private readonly IPetService _petService;

        public HomeController(IAppointmentService appointmentService,
                              IClientService clientService,
                              IPetService petService)
        {
            _appointmentService = appointmentService;
            _clientService = clientService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(10),
                TotalClients = (await _clientService.GetAllActiveAsync()).Count(),
                TotalPets = (await _petService.GetAllAsync()).Count()
            };

            // Role-based View
            if (User.IsInRole("Client"))
            {
                return View("ClientDashboard", model);   // For pet owners
            }

            return View("Dashboard", model); // Staff / Admin dashboard
        }
    }
}