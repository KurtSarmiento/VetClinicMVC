using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Services.Implementations
{
    public class UserLinkService : IUserLinkService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IClientService _clientService;
        private readonly IStaffService _staffService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserLinkService(UserManager<IdentityUser> userManager,
                               IClientService clientService,
                               IStaffService staffService,
                               IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _clientService = clientService;
            _staffService = staffService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LinkCurrentUserToProfileAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            if (user == null) return;

            var userId = user.Id;

            // Auto-link as Client
            if (_httpContextAccessor.HttpContext.User.IsInRole("Client"))
            {
                var existing = (await _clientService.GetAllActiveAsync())
                    .FirstOrDefault(c => c.UserId == userId);

                if (existing == null)
                {
                    var client = new Client
                    {
                        UserId = userId,
                        FirstName = user.Email?.Split('@')[0] ?? "Client",
                        LastName = "",
                        Email = user.Email,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _clientService.AddClientAsync(client, userId);
                }
            }

            // You can add similar logic for Staff later
        }

        public async Task<Client?> GetCurrentClientAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            if (user == null) return null;

            var clients = await _clientService.GetAllActiveAsync();
            return clients.FirstOrDefault(c => c.UserId == user.Id);
        }

        public async Task<Staff?> GetCurrentStaffAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext!.User);
            if (user == null) return null;

            var staffList = await _staffService.GetAllActiveAsync();
            return staffList.FirstOrDefault(s => s.UserId == user.Id);
        }
    }
}