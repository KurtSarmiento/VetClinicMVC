// Controllers/PetController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.ViewModels;

namespace VetClinicMVC.Controllers
{
    [Authorize]
    public class PetController : Controller
    {
        private readonly IPetService _petService;
        private readonly IClientService _clientService;
        private readonly IUserLinkService _userLinkService;

        public PetController(IPetService petService,
                           IClientService clientService,
                           IUserLinkService userLinkService)
        {
            _petService = petService;
            _clientService = clientService;
            _userLinkService = userLinkService;
        }

        public async Task<IActionResult> Create()
        {
            var currentClient = await _userLinkService.GetCurrentClientAsync();

            if (User.IsInRole("Client") && currentClient != null)
            {
                ViewBag.ClientId = currentClient.ClientId;
                ViewBag.ClientName = $"{currentClient.FirstName} {currentClient.LastName}";
            }
            else
            {
                ViewBag.Clients = await _clientService.GetAllActiveAsync();
            }

            ViewBag.Species = await _petService.GetAllSpeciesAsync();
            return View(new PetCreateViewModel());
        }

        [HttpGet]
        public async Task<JsonResult> GetBreedsBySpecies(int speciesId)
        {
            var breeds = await _petService.GetBreedsBySpeciesAsync(speciesId);
            return Json(breeds.Select(b => new {
                breedId = b.BreedId,
                name = b.Name
            }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pet = new Pet
                {
                    ClientId = model.ClientId,
                    Name = model.Name,
                    SpeciesId = model.SpeciesId,
                    BreedId = model.BreedId,
                    Gender = model.Gender,
                    BirthDate = model.BirthDate,
                    Weight = model.Weight,
                    Color = model.Color,
                    MicrochipId = model.MicrochipId ?? $"AUTO-{DateTime.UtcNow.Ticks}", // ← Fix here
                    IsNeutered = model.IsNeutered,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                var currentUserId = User.Identity?.Name ?? "System";
                await _petService.AddPetAsync(pet, currentUserId);

                TempData["Success"] = "Pet added successfully!";

                return User.IsInRole("Client")
                    ? RedirectToAction("MyPets", "ClientPortal")
                    : RedirectToAction("Index");
            }

            // Repopulate on error
            ViewBag.Species = await _petService.GetAllSpeciesAsync();
            if (!User.IsInRole("Client"))
                ViewBag.Clients = await _clientService.GetAllActiveAsync();

            return View(model);
        }
    }
}