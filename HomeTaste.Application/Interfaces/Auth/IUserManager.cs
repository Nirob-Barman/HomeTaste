using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Domain.Entities;

namespace HomeTaste.Application.Interfaces.Auth
{
    public interface IUserManager
    {
        Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByIdAsync(string id);
        Task<string[]> GetRolesAsync(ApplicationUser user);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);        
        Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(ApplicationUser user, string roleName);
        Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(ApplicationUser user, string roleName);

        /// <summary>Updates FirstName, LastName, DateOfBirth, PhoneNumber, ProfileImageUrl, and ProfileImagePublicId.</summary>
        Task<(bool Succeeded, List<string> Errors)> UpdateAsync(ApplicationUser user);

        /// <summary>Changes the password after verifying the current password via ASP.NET Identity.</summary>
        Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

        /// <summary>Returns a page of users optionally filtered by email, first name, or last name.</summary>
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchTerm);

        /// <summary>Returns the total user count, filtered by the same searchTerm as GetAllUsersAsync.</summary>
        Task<int> GetUsersCountAsync(string? searchTerm);

        /// <summary>Fetches multiple users by their identity IDs in a single query.</summary>
        Task<IEnumerable<ApplicationUser>> GetUsersByIdsAsync(IEnumerable<string> userIds);

        /// <summary>Locks (ban) or unlocks (unban) a user. Lock sets LockoutEnd to MaxValue; unlock sets it to the past.</summary>
        Task<(bool Succeeded, List<string> Errors)> SetLockoutAsync(string userId, bool lockout);
    }
}
