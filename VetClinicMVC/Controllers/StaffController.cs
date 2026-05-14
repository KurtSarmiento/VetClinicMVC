using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Added for Include
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.Data; // Ensure this points to your DbContext namespace

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
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

        public async Task<IActionResult> ManageAppointments(string status = "All", DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = status == "Pending"
                    ? query.Where(a => a.Status == "Pending" || a.Status == "Scheduled")
                    : query.Where(a => a.Status == status);
            }

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            ViewBag.Status = status;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");

            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var allowedStatuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
            if (!allowedStatuses.Contains(status))
            {
                TempData["Error"] = "Invalid appointment status.";
                return RedirectToAction(nameof(ManageAppointments));
            }

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
            if (string.IsNullOrWhiteSpace(staff.UserId))
            {
                staff.UserId = $"staff-{Guid.NewGuid():N}";
            }

            staff.CreatedAt ??= DateTime.UtcNow;
            staff.HireDate ??= DateOnly.FromDateTime(DateTime.Today);
            ModelState.Remove(nameof(Staff.UserId));

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
            ModelState.Remove(nameof(Staff.UserId));

            if (ModelState.IsValid)
            {
                var existingStaff = await _context.Staff.AsNoTracking().FirstOrDefaultAsync(s => s.StaffId == staff.StaffId);
                if (existingStaff == null) return NotFound();

                staff.UserId = existingStaff.UserId;
                staff.CreatedAt = existingStaff.CreatedAt;

                var currentUserId = User.Identity?.Name ?? "System";
                await _staffService.UpdateStaffAsync(staff, currentUserId);
                TempData["Success"] = "Staff updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return NotFound();

            staff.IsActive = false;
            _context.Update(staff);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Staff member deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Staff/CheckIn/5
        public async Task<IActionResult> CheckIn(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Pet)
                .Include(a => a.Client)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckIn(
            int appointmentId,
            string chiefComplaint,
            string diagnosis,
            string treatment,
            string? notes,
            decimal? weight,
            decimal? temperature)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(diagnosis) || string.IsNullOrWhiteSpace(treatment))
            {
                TempData["Error"] = "Diagnosis and treatment are required before completing a visit.";
                return RedirectToAction(nameof(CheckIn), new { id = appointmentId });
            }

            var visit = new Visit
            {
                AppointmentId = appointment.AppointmentId,
                PetId = appointment.PetId,
                StaffId = appointment.StaffId,
                VisitDate = DateTime.Now,
                ChiefComplaint = chiefComplaint,
                Diagnosis = diagnosis,
                Treatment = treatment,
                Notes = notes,
                Weight = weight,
                Temperature = temperature
            };

            appointment.Status = "Completed";
            appointment.Notes = string.IsNullOrWhiteSpace(notes)
                ? appointment.Notes
                : $"{appointment.Notes}\n\nVisit notes: {notes}".Trim();

            _context.Visits.Add(visit);
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Medical record saved for {appointment.Pet?.Name ?? "pet"} and appointment completed.";
            return RedirectToAction(nameof(ManageAppointments));
        }
    }
}
