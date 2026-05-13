using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicMVC.Services.Interfaces;
using VetClinicMVC.Models;

namespace VetClinicMVC.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;

        public BillingController(IInvoiceService invoiceService, IPaymentService paymentService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
        }

        // List all invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        // Show Invoice Details & Payment Form
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoicesByClientIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // Handle the Payment Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int invoiceId, decimal amount, string method)
        {
            var currentUserId = User.Identity?.Name ?? "System";

            // 1. Create the payment record via PaymentService
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now
            };

            await _paymentService.AddPaymentAsync(payment, currentUserId);

            // 2. Update invoice status via InvoiceService
            await _invoiceService.MarkAsPaidAsync(invoiceId, currentUserId);

            TempData["Success"] = "Payment processed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}