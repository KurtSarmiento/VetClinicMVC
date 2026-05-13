using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Species
{
    public int SpeciesId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Breed> Breeds { get; set; } = new List<Breed>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
