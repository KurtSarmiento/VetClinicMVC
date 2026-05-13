using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VetClinicMVC.Models;
using VetClinicMVC.Services.Interfaces;

namespace VetClinicMVC.Extensions
{
    public static class UserExtensions
    {
        public static async Task<Staff?> GetCurrentStaffAsync(this UserManager<IdentityUser> userManager,
            IStaffService staffService, ClaimsPrincipal user)
        {
            var userId = userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId)) return null;

            // You can improve this query later
            var staff = (await staffService.GetAllActiveAsync())
                        .FirstOrDefault(s => s.UserId == userId);

            return staff;
        }
    }
}