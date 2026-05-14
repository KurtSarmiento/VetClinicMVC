using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientPortalController : Controller
    {
        private readonly IUserLinkService _userLinkService;
        private readonly IPetService _petService;
        private readonly IAppointmentService _appointmentService;
        private readonly IVisitService _visitService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public ClientPortalController(IUserLinkService userLinkService,
            IPetService petService,
            IAppointmentService appointmentService,
            IVisitService visitService,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager)
        {
            _userLinkService = userLinkService;
            _petService = petService;
            _appointmentService = appointmentService;
            _visitService = visitService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;

            // 1. Get the client
            var clients = await _unitOfWork.Clients.FindAsync(c => c.Email == userEmail);
            var client = clients.FirstOrDefault();

            // 2. If no record exists, go to CompleteProfile
            if (client == null)
            {
                return RedirectToAction("CompleteProfile");
            }

            // 3. Get the stats for the ViewBag
            var pets = await _unitOfWork.Pets.FindAsync(p => p.ClientId == client.ClientId);
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.Pet.ClientId == client.ClientId);

            ViewBag.MyPets = pets;
            ViewBag.MyAppointments = appointments;

            // 🔥 This is the fix: Sending 'client', NOT 'pets'
            return View(client);
        }

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            return View(new Client());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(Client client)
        {
            // 1. Get the current logged-in Identity User
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // 2. STICK THEM TOGETHER!
                client.UserId = user.Id;    // This fills the NULL column
                client.Email = user.Email;  // This ensures the emails match
            }

            ModelState.Remove("UserId"); // Remove from validation if necessary
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                await _unitOfWork.Clients.AddAsync(client);
                await _unitOfWork.CompleteAsync();

                TempData["Success"] = "Profile linked and saved!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
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
        public async Task<IActionResult> MyAppointments()
        {
            // 1. Get the current logged-in client
            var client = await _userLinkService.GetCurrentClientAsync();
            if (client == null) return RedirectToAction("Login", "Account");

            // 2. Fetch their appointments (make sure your service includes Pet and Staff data)
            var appointments = await _appointmentService.GetAppointmentsByClientIdAsync(client.ClientId);

            // 3. Sort by date (newest first)
            return View(appointments.OrderByDescending(a => a.AppointmentDate).ToList());
        }
    }
}