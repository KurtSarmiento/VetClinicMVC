using VetClinicMVC.Models;
using VetClinicMVC.Repositories.Interfaces;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IAuditLogService _auditLogService;

        public ServiceCatalogService(IServiceRepository serviceRepository, IAuditLogService auditLogService)
        {
            _serviceRepository = serviceRepository;
            _auditLogService = auditLogService;
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _serviceRepository.GetServiceWithCategoryAsync(id);
        }

        public async Task<IEnumerable<Service>> GetAllActiveAsync()
        {
            return await _serviceRepository.GetActiveServicesAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId)
        {
            return await _serviceRepository.GetServicesByCategoryAsync(categoryId);
        }

        public async Task AddServiceAsync(Service service, string currentUserId)
        {
            await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Create", "Service",
                service.ServiceId, $"New service added: {service.Name}");
        }

        public async Task UpdateServiceAsync(Service service, string currentUserId)
        {
            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();

            await _auditLogService.LogActionAsync(currentUserId, "System", "Update", "Service",
                service.ServiceId, $"Service updated: {service.Name}");
        }

        public async Task DeleteServiceAsync(int id, string currentUserId)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service != null)
            {
                _serviceRepository.Remove(service);
                await _serviceRepository.SaveChangesAsync();

                await _auditLogService.LogActionAsync(currentUserId, "System", "Delete", "Service",
                    id, $"Service deleted: {service.Name}");
            }
        }
    }
}