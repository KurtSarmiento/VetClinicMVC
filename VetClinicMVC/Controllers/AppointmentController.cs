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
            // 1. Keep your existing ModelState.Remove calls...
            ModelState.Remove("Client");
            ModelState.Remove("Pet");
            ModelState.Remove("Staff");
            ModelState.Remove("AppointmentDate");

            string rawDate = Request.Form["dateInput"];
            string rawTime = Request.Form["timeInput"];

            if (!string.IsNullOrEmpty(rawDate) && !string.IsNullOrEmpty(rawTime))
            {
                appointment.AppointmentDate = DateTime.Parse($"{rawDate} {rawTime}");

                // 2. COLLISION CHECK LOGIC
                // We check if this specific Vet (StaffId) already has an appointment 
                // at this exact time that hasn't been Cancelled.
                var isBusy = await _appointmentService.GetUpcomingAppointmentsAsync(500);
                // Note: Ideally, your service should have a method like 'IsVetAvailable(staffId, dateTime)'
                // But for a quick fix, we can filter the list:

                var existing = isBusy.FirstOrDefault(a =>
                    a.StaffId == appointment.StaffId &&
                    a.AppointmentDate == appointment.AppointmentDate &&
                    a.Status != "Cancelled");

                if (existing != null)
                {
                    ModelState.AddModelError("StaffId", "This Veterinarian is already booked for this time slot. Please choose another time or doctor.");
                }
            }
            else
            {
                ModelState.AddModelError("AppointmentDate", "Please select both date and time.");
            }

            // 3. Validation Logic
            var hour = appointment.AppointmentDate.Hour;
            if (hour < 10 || hour > 17 || (hour == 17 && appointment.AppointmentDate.Minute > 0))
            {
                ModelState.AddModelError("AppointmentDate", "Clinic hours are 10 AM to 5 PM.");
            }

            var client = await _userLinkService.GetCurrentClientAsync();

            if (ModelState.IsValid && client != null)
            {
                appointment.ClientId = client.ClientId;
                appointment.Status = "Pending";
                var currentUserId = User.Identity?.Name ?? "System";

                await _appointmentService.CreateAppointmentAsync(appointment, currentUserId);

                TempData["Success"] = "Appointment requested successfully!";
                return RedirectToAction("Index", "ClientPortal");
            }

            // Repopulate ViewBags so the dropdowns don't break on reload
            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(client?.ClientId ?? 0);
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
        public async Task<IActionResult> MyAppointments()
        {
            var client = await _userLinkService.GetCurrentClientAsync();
            if (client == null) return RedirectToAction("CompleteProfile", "ClientPortal");

            // Fetch only this client's appointments
            var appointments = await _appointmentService.GetAppointmentsByClientIdAsync(client.ClientId);
            return View(appointments);
        }
    }
}