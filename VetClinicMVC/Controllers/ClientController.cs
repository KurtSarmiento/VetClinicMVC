using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IPetService _petService;

        public ClientController(IClientService clientService, IPetService petService)
        {
            _clientService = clientService;
            _petService = petService;
        }

        public async Task<IActionResult> Index(string search = "")
        {
            IEnumerable<Client> clients;

            if (string.IsNullOrEmpty(search))
                clients = await _clientService.GetAllActiveAsync();
            else
                clients = (await _clientService.GetAllActiveAsync())
                    .Where(c => c.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                c.Email.Contains(search));

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
                var currentUserId = User.Identity?.Name ?? "System"; // Improve this later with UserId
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
    }
}