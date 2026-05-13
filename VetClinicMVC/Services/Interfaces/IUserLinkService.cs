using VetClinicMVC.Models;

namespace VetClinicMVC.Services.Interfaces
{
    public interface IUserLinkService
    {
        Task LinkCurrentUserToProfileAsync();
        Task<Client?> GetCurrentClientAsync();
        Task<Staff?> GetCurrentStaffAsync();
    }
}