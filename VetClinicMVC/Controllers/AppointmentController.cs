using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPetService _petService;

        public AppointmentController(IAppointmentService appointmentService, IPetService petService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(20);
            return View(appointments);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Pets = await _petService.GetAllAsync(); // You can improve with SelectList later
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
            return View(appointment);
        }
    }
}