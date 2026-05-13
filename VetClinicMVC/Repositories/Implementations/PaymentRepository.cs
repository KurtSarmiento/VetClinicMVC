using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(VetClinicContext context) : base(context) { }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .SumAsync(p => p.Amount);
        }
    }
}