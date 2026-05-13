using System;
using System.Collections.Generic;

namespace VetClinicMVC.Models;

public partial class Breed
{
    public int BreedId { get; set; }

    public int SpeciesId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual Species Species { get; set; } = null!;
}
