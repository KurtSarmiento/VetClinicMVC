using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly IClientService _clientService;

        public InvoiceController(IInvoiceService invoiceService,
                               IPaymentService paymentService,
                               IClientService clientService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        public async Task<IActionResult> Create(int? clientId = null)
        {
            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            ViewBag.ClientId = clientId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = User.Identity?.Name ?? "System";
                await _invoiceService.CreateInvoiceAsync(invoice, currentUserId);
                TempData["Success"] = "Invoice created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = await _clientService.GetAllActiveAsync();
            return View(invoice);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            ViewBag.Payments = await _paymentService.GetPaymentsByInvoiceAsync(id);
            ViewBag.TotalPaid = await _paymentService.GetTotalPaidAsync(id);

            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(int invoiceId, decimal amount, string paymentMethod, string? transactionRef)
        {
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                TransactionRef = transactionRef,
                PaymentDate = DateTime.UtcNow
            };

            var currentUserId = User.Identity?.Name ?? "System";
            await _paymentService.AddPaymentAsync(payment, currentUserId);

            TempData["Success"] = "Payment recorded successfully!";
            return RedirectToAction(nameof(Details), new { id = invoiceId });
        }

        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var currentUserId = User.Identity?.Name ?? "System";
            await _invoiceService.MarkAsPaidAsync(id, currentUserId);
            TempData["Success"] = "Invoice marked as fully paid!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}