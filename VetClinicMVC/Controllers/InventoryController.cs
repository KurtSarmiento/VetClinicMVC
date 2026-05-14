using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Veterinarian,Receptionist")]
    public class InventoryController : Controller
    {
        private readonly IMedicationService _medicationService;

        public InventoryController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        // List all medications and stock status
        public async Task<IActionResult> Index()
        {
            var medications = await _medicationService.GetAllMedicationsAsync();
            return View(medications);
        }

        // GET: Create new Medication
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Medication medication)
        {
            if (ModelState.IsValid)
            {
                await _medicationService.AddMedicationAsync(medication);
                TempData["Success"] = "New medication added to inventory.";
                return RedirectToAction(nameof(Index));
            }
            return View(medication);
        }

        // Quick Stock Adjustment Action
        [HttpPost]
        public async Task<IActionResult> QuickAdjust(int id, int amount)
        {
            // If amount is positive, it adds. If negative, it deducts.
            var success = await _medicationService.UpdateStockAsync(id, amount);
            if (success) TempData["Success"] = "Stock updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
