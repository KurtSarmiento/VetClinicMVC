using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Veterinarian")]
    public class VisitController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly IAppointmentService _appointmentService;

        public VisitController(IVisitService visitService, IAppointmentService appointmentService)
        {
            _visitService = visitService;
            _appointmentService = appointmentService;
        }

        // Create a visit record from an appointment
        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentService.GetByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            var visit = new Visit
            {
                AppointmentId = appointmentId,
                PetId = appointment.PetId,
                StaffId = appointment.StaffId,
                VisitDate = DateTime.Now,
                Weight = appointment.Pet?.Weight ?? 0
            };

            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visit visit)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _visitService.CreateVisitAsync(visit, currentUserId);

                // Also update appointment status to Completed
                await _appointmentService.UpdateAppointmentStatusAsync(visit.AppointmentId.Value, "Completed", currentUserId);

                TempData["Success"] = "Medical record saved successfully!";
                return RedirectToAction("Index", "Appointment");
            }
            return View(visit);
        }
    }
}