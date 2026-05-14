// Controllers/PetController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly VetClinicContext _context;

        public PetController(IPetService petService,
                           IClientService clientService,
                           IUserLinkService userLinkService,
                           IUnitOfWork unitOfWork,
                           VetClinicContext context)
        {
            _petService = petService;
            _clientService = clientService;
            _userLinkService = userLinkService;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // We use .Include to "Join" the tables so we can see Names instead of just IDs
            var query = _context.Pets
                .Include(p => p.Client)
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .AsNoTracking();

            if (User.IsInRole("Client"))
            {
                // Filter so clients only see their own pets
                var userEmail = User.Identity?.Name;
                var clientPets = await query
                    .Where(p => p.Client.Email == userEmail)
                    .ToListAsync();
                return View(clientPets);
            }

            // Admin/Staff sees everyone
            var allPets = await query.ToListAsync();
            return View(allPets);
        }

        public async Task<IActionResult> Create(int? clientId = null)
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
            return View(new PetCreateViewModel { ClientId = clientId ?? currentClient?.ClientId ?? 0 });
        }

        [HttpGet]
        public async Task<JsonResult> GetBreedsBySpecies(int speciesId)
        {
            var breeds = await _petService.GetBreedsBySpeciesAsync(speciesId);

            // Ensure the property names are exactly as used in the JS (case-sensitive sometimes)
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
        // GET: Pet/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Client)
                .Include(p => p.Species)
                .Include(p => p.Breed)
                .FirstOrDefaultAsync(p => p.PetId == id);

            if (pet == null) return NotFound();

            return View(pet);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var pet = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.PetId == id);
            if (pet == null) return NotFound();

            await PopulatePetEditListsAsync(pet);
            return View(pet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Pet pet)
        {
            ModelState.Remove(nameof(Pet.Client));
            ModelState.Remove(nameof(Pet.Species));

            if (ModelState.IsValid)
            {
                var existing = await _context.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.PetId == pet.PetId);
                if (existing == null) return NotFound();

                pet.CreatedAt = existing.CreatedAt;
                pet.UpdatedAt = DateTime.UtcNow;

                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Pet updated successfully.";
                return RedirectToAction(nameof(Details), new { id = pet.PetId });
            }

            await PopulatePetEditListsAsync(pet);
            return View(pet);
        }

        private async Task PopulatePetEditListsAsync(Pet pet)
        {
            var clients = await _clientService.GetAllActiveAsync();
            ViewBag.Clients = clients.Select(c => new SelectListItem
            {
                Value = c.ClientId.ToString(),
                Text = $"{c.FirstName} {c.LastName}",
                Selected = c.ClientId == pet.ClientId
            });
            ViewBag.Species = new SelectList(await _petService.GetAllSpeciesAsync(), "SpeciesId", "Name", pet.SpeciesId);
            ViewBag.Breeds = new SelectList(await _petService.GetBreedsBySpeciesAsync(pet.SpeciesId), "BreedId", "Name", pet.BreedId);
        }
    }

}
