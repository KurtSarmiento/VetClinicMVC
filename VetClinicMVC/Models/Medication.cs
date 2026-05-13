using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Medication
{
    public int MedicationId { get; set; }

    public string Name { get; set; } = null!;

    public string? GenericName { get; set; }

    public string? Manufacturer { get; set; }

    public string? Unit { get; set; }

    public decimal? UnitPrice { get; set; }

    public int? StockQuantity { get; set; }

    public int? ReorderLevel { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public bool? IsActive { get; set; }
}
