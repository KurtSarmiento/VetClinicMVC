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
        private readonly IUserLinkService _userLinkService;

        public BillingController(
            IInvoiceService invoiceService,
            IPaymentService paymentService,
            IUserLinkService userLinkService)
        {
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _userLinkService = userLinkService;
        }

        // List all invoices
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Client"))
            {
                return RedirectToAction("MyBills", "ClientPortal");
            }

            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        // Show Invoice Details & Payment Form
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (User.IsInRole("Client"))
            {
                var client = await _userLinkService.GetCurrentClientAsync();
                if (client == null || invoice.ClientId != client.ClientId)
                {
                    return Forbid();
                }
            }

            return View(invoice);
        }

        // Handle the Payment Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
