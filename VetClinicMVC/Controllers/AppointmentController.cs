using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;
        private readonly IStaffService _staffService;
        private readonly IUserLinkService _userLinkService;

        public AppointmentController(IAppointmentService appointmentService,
                                   IPetService petService,
                                   IStaffService staffService,
                                   IUserLinkService userLinkService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _staffService = staffService;
            _userLinkService = userLinkService;
        }

        // STAFF VIEW: List all upcoming appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(50);
            return View(appointments);
        }

        // CLIENT VIEW: Booking Form
        public async Task<IActionResult> Book()
        {
            var currentClient = await _userLinkService.GetCurrentClientAsync();
            if (currentClient == null)
            {
                TempData["Error"] = "Client profile not found. Please complete your profile first.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            ViewBag.Staff = await _staffService.GetVeterinariansAsync();

            // Set a default date for the UI
            var model = new Appointment { AppointmentDate = DateTime.Now.AddDays(1) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                // Ensure the client ID is attached to the appointment
                var client = await _userLinkService.GetCurrentClientAsync();
                appointment.ClientId = client.ClientId;

                await _appointmentService.CreateAppointmentAsync(appointment, currentUserId);

                TempData["Success"] = "Appointment request submitted successfully!";
                return User.IsInRole("Client") ? RedirectToAction("Index", "ClientPortal") : RedirectToAction("Index");
            }

            var currentClient = await _userLinkService.GetCurrentClientAsync();
            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            ViewBag.Staff = await _staffService.GetVeterinariansAsync();
            return View(appointment);
        }

        // STAFF ACTION: Confirm Appointment
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            // Match the name in your Service: UpdateAppointmentStatusAsync
            await _appointmentService.UpdateAppointmentStatusAsync(id, "Confirmed", currentUserId);

            TempData["Success"] = "Appointment Confirmed.";
            return RedirectToAction(nameof(Index));
        }

        // STAFF ACTION: Cancel Appointment
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            // Using your specific Cancel method
            await _appointmentService.CancelAppointmentAsync(id, currentUserId, "Cancelled by staff");

            TempData["Warning"] = "Appointment Cancelled.";
            return RedirectToAction(nameof(Index));
        }
    }
}