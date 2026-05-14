using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Added for Include
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.Data; // Ensure this points to your DbContext namespace

namespace VetClinicMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly IAppointmentService _appointmentService;
        private readonly VetClinicContext _context; // Adding context for quick updates

        public StaffController(IStaffService staffService, IAppointmentService appointmentService, VetClinicContext context)
        {
            _staffService = staffService;
            _appointmentService = appointmentService;
            _context = context;
        }

        // --- APPOINTMENT MANAGEMENT ACTIONS ---

        public async Task<IActionResult> ManageAppointments()
        {
            // Fetching all appointments with related Pet and Client data
            var appointments = await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Appointment marked as {status}.";
            }
            return RedirectToAction(nameof(ManageAppointments));
        }

        // --- EXISTING STAFF MANAGEMENT ACTIONS ---

        public async Task<IActionResult> Index()
        {
            var staff = await _staffService.GetAllActiveAsync();
            return View(staff);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _staffService.AddStaffAsync(staff, currentUserId);
                TempData["Success"] = "Staff member added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        public async Task<IActionResult> Details(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _staffService.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Staff staff)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _staffService.UpdateStaffAsync(staff, currentUserId);
                TempData["Success"] = "Staff updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }
    }
}