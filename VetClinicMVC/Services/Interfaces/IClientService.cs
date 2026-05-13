using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IClientService
    {
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> GetAllActiveAsync();
        Task AddClientAsync(Client client, string currentUserId);
        Task UpdateClientAsync(Client client, string currentUserId);
        Task DeleteClientAsync(int id, string currentUserId);
        Task<Client?> GetClientWithPetsAsync(int clientId);
    }
}