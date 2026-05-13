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

        public AppointmentController(IAppointmentService appointmentService,
                                   IPetService petService,
                                   IStaffService staffService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _staffService = staffService;
        }

        // Staff View - All Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(20);
            return View(appointments);
        }

        // Staff - Create Appointment
        public async Task<IActionResult> Create()
        {
            ViewBag.Pets = await _petService.GetAllAsync();
            ViewBag.Staff = await _staffService.GetVeterinariansAsync(); // Only vets
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _appointmentService.CreateAppointmentAsync(appointment, currentUserId);

                TempData["Success"] = "Appointment scheduled successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Pets = await _petService.GetAllAsync();
            ViewBag.Staff = await _staffService.GetVeterinariansAsync();
            return View(appointment);
        }

        // Client View - My Appointments
        public async Task<IActionResult> MyAppointments()
        {
            // TODO: Filter by current client later
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(10);
            return View(appointments);
        }
    }
}