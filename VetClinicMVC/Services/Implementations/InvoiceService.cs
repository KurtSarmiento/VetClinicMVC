using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAuditLogService _auditLogService;

        public InvoiceService(IInvoiceRepository invoiceRepository, IAuditLogService auditLogService)
        {
            _invoiceRepository = invoiceRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _invoiceRepository.GetInvoiceWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _invoiceRepository.GetAllWithDetailsAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId)
        {
            return await _invoiceRepository.GetInvoicesByClientIdAsync(clientId);
        }

        public async Task CreateInvoiceAsync(Invoice invoice, string currentUserId)
        {
            await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Invoice",
                invoice.InvoiceId, $"Invoice #{invoice.InvoiceNumber} created for client {invoice.ClientId}");
        }

        public async Task MarkAsPaidAsync(int invoiceId, string currentUserId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                _invoiceRepository.Update(invoice);
                await _invoiceRepository.SaveChangesAsync();

                await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Invoice",
                    invoiceId, $"Invoice #{invoice.InvoiceNumber} marked as Paid");
            }
        }
    }
}
