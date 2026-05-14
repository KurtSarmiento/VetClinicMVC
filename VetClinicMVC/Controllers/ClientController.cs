using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    [Authorize(Roles = "Admin,Receptionist,Veterinarian")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IPetService _petService;
        private readonly VetClinicContext _context;

        public ClientController(IClientService clientService, IPetService petService, VetClinicContext context)
        {
            _clientService = clientService;
            _petService = petService;
            _context = context;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            var clients = await _clientService.GetAllActiveAsync();

            if (!string.IsNullOrEmpty(search))
            {
                clients = clients.Where(c =>
                    c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            ViewBag.Search = search;
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _clientService.AddClientAsync(client, currentUserId);

                TempData["Success"] = "Client added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientService.GetClientWithPetsAsync(id);
            if (client == null) return NotFound();

            ViewBag.PetVisits = await _context.Visits
                .Include(v => v.Pet)
                .Include(v => v.Staff)
                .Where(v => v.Pet.ClientId == id)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();

            return View(client);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _clientService.UpdateClientAsync(client, currentUserId);

                TempData["Success"] = "Client updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _clientService.DeleteClientAsync(id, currentUserId);

            TempData["Success"] = "Client deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
