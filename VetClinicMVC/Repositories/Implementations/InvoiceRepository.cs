using Microsoft.EntityFrameworkCore;
using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;

namespace VetClinicMVC.Repositories.Implementations
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(VetClinicContext context) : base(context) { }

        public async Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(int clientId)
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Where(i => i.ClientId == clientId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetUnpaidInvoicesAsync()
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Where(i => i.Status == "Pending" || i.Status == "PartiallyPaid")
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllWithDetailsAsync()
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Visit)
                    .ThenInclude(v => v!.Pet)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }
    }
}
