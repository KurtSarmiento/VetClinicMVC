// Controllers/ClientPortalController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientPortalController : Controller
    {
        private readonly IUserLinkService _userLinkService;
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;
        private readonly IVisitService _visitService;

        public ClientPortalController(IUserLinkService userLinkService,
            IPetService petService,
            IAppointmentService appointmentService,
            IVisitService visitService)
        {
            _userLinkService = userLinkService;
            _petService = petService;
            _appointmentService = appointmentService;
            _visitService = visitService;
        }

        public async Task<IActionResult> Index()
        {
            await _userLinkService.LinkCurrentUserToProfileAsync();

            var currentClient = await _userLinkService.GetCurrentClientAsync();
            if (currentClient == null)
            {
                TempData["Error"] = "Your account is not linked yet. Please contact admin.";
                return RedirectToAction("Index", "Home");
            }

            var myPets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            var myAppointments = await _appointmentService.GetAppointmentsByClientIdAsync(currentClient.ClientId);

            ViewBag.MyPets = myPets;
            ViewBag.MyAppointments = myAppointments;

            return View(currentClient);
        }

        public async Task<IActionResult> MyPets()
        {
            var currentClient = await _userLinkService.GetCurrentClientAsync();
            if (currentClient == null) return RedirectToAction("Index", "Home");

            var pets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            return View(pets);
        }

        public async Task<IActionResult> MyVisits()
        {
            var currentClient = await _userLinkService.GetCurrentClientAsync();
            if (currentClient == null) return RedirectToAction("Index", "Home");

            var visits = await _visitService.GetVisitsByClientIdAsync(currentClient.ClientId);
            return View(visits);
        }

        public async Task<IActionResult> BookAppointment()
        {
            var currentClient = await _userLinkService.GetCurrentClientAsync();
            if (currentClient == null) return RedirectToAction("Index", "Home");

            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            return View();
        }
    }
}