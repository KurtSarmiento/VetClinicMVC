using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int ClientId { get; set; }

    public int? VisitId { get; set; }

    public string InvoiceNumber { get; set; } = null!;

    public DateTime? IssueDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public decimal Subtotal { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Visit? Visit { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
