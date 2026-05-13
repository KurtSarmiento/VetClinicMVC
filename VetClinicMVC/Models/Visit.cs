using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Visit
{
    public int VisitId { get; set; }

    public int? AppointmentId { get; set; }

    public int PetId { get; set; }

    public int StaffId { get; set; }

    public DateTime? VisitDate { get; set; }

    public string? ChiefComplaint { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Notes { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Temperature { get; set; }

    public bool? IsFollowUp { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Pet Pet { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
