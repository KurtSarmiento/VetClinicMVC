using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PetId { get; set; }

    public int ClientId { get; set; }

    public int StaffId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int? DurationMinutes { get; set; }

    public string? Status { get; set; }

    public string? Reason { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
