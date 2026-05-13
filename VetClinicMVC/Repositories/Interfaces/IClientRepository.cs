using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<Client?> GetClientWithPetsAsync(int clientId);
        Task<IEnumerable<Client>> GetActiveClientsAsync();
        Task<Client?> GetByEmailAsync(string email);
    }
}