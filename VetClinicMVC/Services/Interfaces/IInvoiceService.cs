using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId);
        Task CreateInvoiceAsync(Invoice invoice, string currentUserId);
        Task MarkAsPaidAsync(int invoiceId, string currentUserId);
    }
}