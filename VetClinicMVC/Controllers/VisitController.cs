using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Veterinarian")]
    public class VisitController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly IAppointmentService _appointmentService;
        private readonly VetClinicContext _context;

        public VisitController(IVisitService visitService, IAppointmentService appointmentService, VetClinicContext context)
        {
            _visitService = visitService;
            _appointmentService = appointmentService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var visits = await _visitService.GetRecentVisitsAsync(50);
            return View(visits);
        }

        public async Task<IActionResult> Details(int id)
        {
            var visit = await _visitService.GetByIdAsync(id);
            if (visit == null)
            {
                return NotFound();
            }

            return View(visit);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var visit = await _visitService.GetByIdAsync(id);
            if (visit == null) return NotFound();
            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Visit visit)
        {
            ModelState.Remove(nameof(Visit.Pet));
            ModelState.Remove(nameof(Visit.Staff));
            ModelState.Remove(nameof(Visit.Appointment));

            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _visitService.UpdateVisitAsync(visit, currentUserId);
                TempData["Success"] = "Medical record updated.";
                return RedirectToAction(nameof(Details), new { id = visit.VisitId });
            }

            return View(visit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var visit = await _context.Visits.FirstOrDefaultAsync(v => v.VisitId == id);
            if (visit == null) return NotFound();

            _context.Visits.Remove(visit);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Medical record deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Create a visit record from an appointment
        public async Task<IActionResult> Create(int appointmentId)
        {
            if (appointmentId <= 0)
            {
                TempData["Error"] = "Please choose an appointment before recording a medical visit.";
                return RedirectToAction("Index", "Appointment");
            }

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
                if (!visit.AppointmentId.HasValue)
                {
                    ModelState.AddModelError("AppointmentId", "A visit must be linked to an appointment.");
                    return View(visit);
                }

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
