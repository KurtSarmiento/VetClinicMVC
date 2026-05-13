using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;   // We'll create this service next if you want

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _staffService.GetAllActiveAsync();
            return View(staff);
        }

        public IActionResult Create()
        {
            return View();
        }

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