using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IPaymentService
    {
        Task AddPaymentAsync(Payment payment, string currentUserId);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceAsync(int invoiceId);
        Task<decimal> GetTotalPaidAsync(int invoiceId);
    }
}