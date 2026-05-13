using VetClinicMVC.Models;

namespace VetClinicMVC.Repositories.Interfaces
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId);
        Task<Service?> GetServiceWithCategoryAsync(int serviceId);
    }
}