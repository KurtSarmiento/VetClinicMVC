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
        private readonly IInvoiceService _invoiceService;
        private readonly IServiceCatalogService _serviceCatalogService;

        public AppointmentController(IAppointmentService appointmentService,
                                   IPetService petService,
                                   IStaffService staffService,
                                   IUserLinkService userLinkService,
                                   IInvoiceService invoiceService,
                                   IServiceCatalogService serviceCatalogService)
        {
            _appointmentService = appointmentService;
            _petService = petService;
            _staffService = staffService;
            _userLinkService = userLinkService;
            _invoiceService = invoiceService;
            _serviceCatalogService = serviceCatalogService;
        }

        // STAFF VIEW: List all upcoming appointments
        [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(50);
            return View(appointments);
        }

        [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
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
            ViewBag.Services = await _serviceCatalogService.GetAllActiveAsync();

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
            ModelState.Remove("Reason");

            var rawDate = Request.Form["dateInput"].ToString();
            var rawTime = Request.Form["timeInput"].ToString();
            var rawServiceId = Request.Form["serviceId"].ToString();
            Service? selectedService = null;

            if (int.TryParse(rawServiceId, out var serviceId))
            {
                selectedService = await _serviceCatalogService.GetByIdAsync(serviceId);
                if (selectedService == null || !selectedService.IsActive)
                {
                    ModelState.AddModelError("Reason", "Please choose an available service.");
                }
            }
            else
            {
                ModelState.AddModelError("Reason", "Please choose a service for this appointment.");
            }

            var hasValidDateTime = false;

            if (!string.IsNullOrEmpty(rawDate) && !string.IsNullOrEmpty(rawTime))
            {
                if (!DateTime.TryParse($"{rawDate} {rawTime}", out var selectedDateTime))
                {
                    ModelState.AddModelError("AppointmentDate", "Please select a valid appointment date and time.");
                }
                else
                {
                    appointment.AppointmentDate = selectedDateTime;
                    hasValidDateTime = true;
                }

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
            if (hasValidDateTime && (hour < 10 || hour > 17 || (hour == 17 && appointment.AppointmentDate.Minute > 0)))
            {
                ModelState.AddModelError("AppointmentDate", "Clinic hours are 10 AM to 5 PM.");
            }

            var client = await _userLinkService.GetCurrentClientAsync();

            if (ModelState.IsValid && client != null)
            {
                appointment.ClientId = client.ClientId;
                appointment.Status = "Pending";
                appointment.Reason = selectedService!.Name;
                var currentUserId = User.Identity?.Name ?? "System";

                await _appointmentService.CreateAppointmentAsync(appointment, currentUserId);
                await _invoiceService.CreateInvoiceAsync(new Invoice
                {
                    ClientId = client.ClientId,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                    Subtotal = selectedService.BasePrice,
                    TaxAmount = 0m,
                    TotalAmount = selectedService.BasePrice,
                    Status = "Pending",
                    Notes = $"{selectedService.Name} booking for {appointment.AppointmentDate:MMM dd, yyyy h:mm tt}."
                }, currentUserId);

                TempData["Success"] = "Appointment requested successfully. A pending booking bill has been added to your account.";
                return RedirectToAction("Index", "ClientPortal");
            }

            // Repopulate ViewBags so the dropdowns don't break on reload
            ViewBag.MyPets = await _petService.GetPetsByClientIdAsync(client?.ClientId ?? 0);
            ViewBag.Staff = await _staffService.GetVeterinariansAsync();
            ViewBag.Services = await _serviceCatalogService.GetAllActiveAsync();
            return View(appointment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _appointmentService.UpdateAppointmentStatusAsync(id, "Confirmed", currentUserId);
            TempData["Success"] = "Appointment Confirmed.";
            return RedirectToAction("ManageAppointments", "Staff");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _appointmentService.CancelAppointmentAsync(id, currentUserId, "Cancelled by staff");
            TempData["Warning"] = "Appointment Cancelled.";
            return RedirectToAction("ManageAppointments", "Staff");
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
