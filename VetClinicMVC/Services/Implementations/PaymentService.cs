using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAuditLogService _auditLogService;

        public PaymentService(IPaymentRepository paymentRepository, IAuditLogService auditLogService)
        {
            _paymentRepository = paymentRepository;
            _auditLogService = auditLogService;
        }

        public async Task AddPaymentAsync(Payment payment, string currentUserId)
        {
            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Payment",
                payment.PaymentId, $"Payment of {payment.Amount} received via {payment.PaymentMethod}");
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceAsync(int invoiceId)
        {
            return await _paymentRepository.GetPaymentsByInvoiceIdAsync(invoiceId);
        }

        public async Task<decimal> GetTotalPaidAsync(int invoiceId)
        {
            return await _paymentRepository.GetTotalPaidAmountAsync(invoiceId);
        }
    }
}