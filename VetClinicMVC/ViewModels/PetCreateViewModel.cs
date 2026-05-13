// ViewModels/PetCreateViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace VetClinicMVC.ViewModels
{
    public class PetCreateViewModel
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Pet name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Species is required")]
        public int SpeciesId { get; set; }

        public int? BreedId { get; set; }

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        public decimal? Weight { get; set; }

        public string? Color { get; set; }

        public bool IsNeutered { get; set; } = false;

        public string? Notes { get; set; }
    }
}