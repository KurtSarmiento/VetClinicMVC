using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string UserId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string? LicenseNumber { get; set; }

    public string? Specialization { get; set; }

    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
