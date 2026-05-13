using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class VisitController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly IPetService _petService;

        public VisitController(IVisitService visitService, IPetService petService)
        {
            _visitService = visitService;
            _petService = petService;
        }

        public async Task<IActionResult> Index()
        {
            var visits = await _visitService.GetRecentVisitsAsync(30);
            return View(visits);
        }

        public async Task<IActionResult> Create(int? petId = null)
        {
            ViewBag.Pets = await _petService.GetAllAsync();
            ViewBag.PetId = petId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visit visit)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _visitService.CreateVisitAsync(visit, currentUserId);
                TempData["Success"] = "Visit recorded successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Pets = await _petService.GetAllAsync();
            return View(visit);
        }

        public async Task<IActionResult> Details(int id)
        {
            var visit = await _visitService.GetByIdAsync(id);
            if (visit == null) return NotFound();
            return View(visit);
        }
    }
}