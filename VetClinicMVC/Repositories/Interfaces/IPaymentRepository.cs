using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);
        Task<decimal> GetTotalPaidAmountAsync(int invoiceId);
    }
}