using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.ViewModels;

namespace VetClinicMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IClientService _clientService;
        private readonly IPetService _petService;
        private readonly IVisitService _visitService;

        public HomeController(IAppointmentService appointmentService,
                              IClientService clientService,
                              IPetService petService,
                              IVisitService visitService)
        {
            _appointmentService = appointmentService;
            _clientService = clientService;
            _petService = petService;
            _visitService = visitService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                UpcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(8),
                TotalClients = (await _clientService.GetAllActiveAsync()).Count(),
                TotalPets = (await _petService.GetAllAsync()).Count(),
                RecentVisits = await _visitService.GetRecentVisitsAsync(6),
            };

            return View(model);
        }
    }
}