using System.ComponentModel.DataAnnotations;

namespace VetClinicMVC.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int InvoiceId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionRef { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Invoice Invoice { get; set; } = null!;
    }
}