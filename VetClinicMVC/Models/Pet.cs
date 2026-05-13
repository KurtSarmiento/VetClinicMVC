using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Pet
{
    public int PetId { get; set; }

    public int ClientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SpeciesId { get; set; }

    public int? BreedId { get; set; }

    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public decimal? Weight { get; set; }

    public string? Color { get; set; }

    public string? MicrochipId { get; set; }

    public bool IsNeutered { get; set; } = false;

    public string? Notes { get; set; }

    public string? PhotoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeceased { get; set; } = false;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Breed? Breed { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Species Species { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
