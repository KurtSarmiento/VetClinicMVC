using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public class PetController : Controller
    {
        private readonly IPetService _petService;
        private readonly IClientService _clientService;

        public PetController(IPetService petService, IClientService clientService)
        {
            _petService = petService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            var pets = await _petService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                pets = pets.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Client.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Client.LastName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return View(pets);
        }

        public async Task<IActionResult> Create(int? clientId = null)
        {
            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            ViewBag.ClientId = clientId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pet pet)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _petService.AddPetAsync(pet, currentUserId);

                TempData["Success"] = "Pet added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            return View(pet);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return View(pet);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();

            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pet pet)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _petService.UpdatePetAsync(pet, currentUserId);

                TempData["Success"] = "Pet updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            return View(pet);
        }
    }
}