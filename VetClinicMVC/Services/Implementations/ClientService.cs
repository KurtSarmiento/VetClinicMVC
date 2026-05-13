using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly IAuditLogService _auditLogService;

    public ClientService(IClientRepository clientRepository, IAuditLogService auditLogService)
    {
        _clientRepository = clientRepository;
        _auditLogService = auditLogService;
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _clientRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Client>> GetAllActiveAsync()
    {
        return await _clientRepository.GetActiveClientsAsync();
    }

    public async Task AddClientAsync(Client client, string currentUserId)
    {
        await _clientRepository.AddAsync(client);
        await _clientRepository.SaveChangesAsync();   // ← Fixed

        await _auditLogService.LogActionAsync(
            currentUserId,
            "System",
            "Create",
            "Client",
            client.ClientId,
            $"New client added: {client.FirstName} {client.LastName}");
    }

    public async Task UpdateClientAsync(Client client, string currentUserId)
    {
        _clientRepository.Update(client);
        await _clientRepository.SaveChangesAsync();

        await _auditLogService.LogActionAsync(
            currentUserId,
            "System",
            "Update",
            "Client",
            client.ClientId,
            $"Client updated: {client.FirstName} {client.LastName}");
    }

    public async Task DeleteClientAsync(int id, string currentUserId)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        if (client != null)
        {
            _clientRepository.Remove(client);
            await _clientRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(
                currentUserId,
                "System",
                "Delete",
                "Client",
                id,
                $"Client deleted: {client.FirstName} {client.LastName}");
        }
    }

    public async Task<Client?> GetClientWithPetsAsync(int clientId)
    {
        return await _clientRepository.GetClientWithPetsAsync(clientId);
    }
}