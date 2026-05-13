using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId);
        Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync();
        Task<Invoice?> GetInvoiceWithDetailsAsync(int id);
    }
}