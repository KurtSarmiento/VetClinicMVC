using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public int? DurationMinutes { get; set; }

    public bool IsActive { get; set; } = false;

    public virtual ServiceCategory Category { get; set; } = null!;
}
