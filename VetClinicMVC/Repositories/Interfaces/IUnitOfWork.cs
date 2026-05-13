using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Add all your repositories here so the Services can find them
        IGenericRepository<Client> Clients { get; }
        IGenericRepository<Pet> Pets { get; }
        IGenericRepository<Appointment> Appointments { get; }
        IGenericRepository<Medication> Medications { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<Visit> Visits { get; }

        Task<int> CompleteAsync(); // This is the "Save" button for the whole DB
    }
}