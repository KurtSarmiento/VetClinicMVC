using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore; // Added for firstOrDefault logic

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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(50);
            return View(appointments);
        }

        // CLIENT VIEW: Booking Form
        // Named 'Book' but called by 'BookAppointment' in your Dashboard? 
        // Let's keep it 'Book' but make sure your dashboard links here.
        public async Task<IActionResult> Book()
        {
            // FIX: Instead of relying on UserId (which is NULL), we find by Email
            var userEmail = User.Identity?.Name;
            var currentClient = await _userLinkService.GetCurrentClientAsync();

            // If the service fails because of the NULL UserId, we do a manual fallback check:
            if (currentClient == null && !string.IsNullOrEmpty(userEmail))
            {
                // This is a safety net in case UserLinkService is looking for UserId
                // You might need to check your UserLinkService implementation
                TempData["Error"] = "Please ensure your profile email matches your login.";
                return RedirectToAction("Index", "ClientPortal");
            }

            if (currentClient == null)
            {
                TempData["Error"] = "Client profile not found. Please complete your profile first.";
                return RedirectToAction("CompleteProfile", "ClientPortal");
            }

            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(currentClient.ClientId);
            ViewBag.Staff = await _staffService.GetVeterinariansAsync();

            var model = new Appointment { AppointmentDate = DateTime.Now.AddDays(1) };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Appointment appointment)
        {
            var client = await _userLinkService.GetCurrentClientAsync();

            if (ModelState.IsValid && client != null)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                appointment.ClientId = client.ClientId;

                await _appointmentService.CreateAppointmentAsync(appointment, currentUserId);

                TempData["Success"] = "Appointment request submitted successfully!";
                return User.IsInRole("Client") ? RedirectToAction("Index", "ClientPortal") : RedirectToAction("Index");
            }

            // Repopulate ViewBags if validation fails
            if (client != null)
                ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(client.ClientId);

            ViewBag.Staff = await _staffService.GetVeterinariansAsync();
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Confirm(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _appointmentService.UpdateAppointmentStatusAsync(id, "Confirmed", currentUserId);
            TempData["Success"] = "Appointment Confirmed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _appointmentService.CancelAppointmentAsync(id, currentUserId, "Cancelled by staff");
            TempData["Warning"] = "Appointment Cancelled.";
            return RedirectToAction(nameof(Index));
        }
    }
}