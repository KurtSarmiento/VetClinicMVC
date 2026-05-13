using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class ServiceCatalogController : Controller
    {
        private readonly IServiceCatalogService _serviceCatalogService;

        public ServiceCatalogController(IServiceCatalogService serviceCatalogService)
        {
            _serviceCatalogService = serviceCatalogService;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _serviceCatalogService.GetAllActiveAsync();
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _serviceCatalogService.AddServiceAsync(service, currentUserId);
                TempData["Success"] = "Service added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
    }
}