using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IServiceCatalogService
    {
        Task<Service?> GetByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllActiveAsync();
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId);

        Task AddServiceAsync(Service service, string currentUserId);
        Task UpdateServiceAsync(Service service, string currentUserId);
        Task DeleteServiceAsync(int id, string currentUserId);
    }
}