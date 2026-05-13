using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Client")]   // Only logged-in Clients
    public class ClientPortalController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;

        public ClientPortalController(IClientService clientService,
                                    IPetService petService,
                                    IAppointmentService appointmentService)
        {
            _clientService = clientService;
            _petService = petService;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.Name; // or better: UserManager.GetUserId(User)

            // TODO: Later link UserId to Client record
            var pets = await _petService.GetAllAsync(); // Temporary - will filter by owner later

            ViewBag.MyAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(5);

            return View(pets);
        }

        public async Task<IActionResult> MyPets()
        {
            var pets = await _petService.GetAllAsync(); // Improve later with proper filtering
            return View(pets);
        }

        public async Task<IActionResult> BookAppointment()
        {
            ViewBag.Pets = await _petService.GetAllAsync();
            return View();
        }
    }
}