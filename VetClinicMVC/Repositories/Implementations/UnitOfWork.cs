using VetClinicMVC.Data;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VetClinicContext _context;

        public UnitOfWork(VetClinicContext context)
        {
            _context = context;
            Clients = new GenericRepository<Client>(_context);
            Pets = new GenericRepository<Pet>(_context);
            Appointments = new GenericRepository<Appointment>(_context);
            Medications = new GenericRepository<Medication>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            Payments = new GenericRepository<Payment>(_context);
            Visits = new GenericRepository<Visit>(_context);
        }

        public IGenericRepository<Client> Clients { get; private set; }
        public IGenericRepository<Pet> Pets { get; private set; }
        public IGenericRepository<Appointment> Appointments { get; private set; }
        public IGenericRepository<Medication> Medications { get; private set; }
        public IGenericRepository<Invoice> Invoices { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<Visit> Visits { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}