using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Domain.Entities;
using HomeTaste.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Infrastructure.Identity
{
    public class IdentityUserManager : IUserManager
    {
        private readonly UserManager<IdentityApplicationUser> _userManager;

        public IdentityUserManager(UserManager<IdentityApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ── private helper ───────────────────────────────────────────────────────

        private async Task<ApplicationUser> MapAsync(IdentityApplicationUser u)
        {
            var lockoutEnd = await _userManager.GetLockoutEndDateAsync(u);
            return new ApplicationUser
            {
                Id                   = u.Id,
                Email                = u.Email!,
                FirstName            = u.FirstName,
                LastName             = u.LastName,
                DateOfBirth          = u.DateOfBirth,
                PhoneNumber          = u.PhoneNumber,
                ProfileImageUrl      = u.ProfileImageUrl,
                ProfileImagePublicId = u.ProfileImagePublicId,
                IsLocked             = lockoutEnd.HasValue && lockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }

        // ── existing methods ─────────────────────────────────────────────────────

        public async Task<(bool Succeeded, string? UserId, List<string> Errors)> CreateAsync(ApplicationUser user, string password)
        {
            var identityUser = new IdentityApplicationUser
            {
                Email       = user.Email,
                UserName    = user.Email,
                FirstName   = user.FirstName,
                LastName    = user.LastName,
                DateOfBirth = user.DateOfBirth,
            };

            var result = await _userManager.CreateAsync(identityUser, password);
            return result.Succeeded
                ? (true, identityUser.Id, new List<string>())
                : (false, null, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            var u = await _userManager.FindByEmailAsync(email);
            return u == null ? null : await MapAsync(u);
        }

        public async Task<ApplicationUser?> FindByIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            return u == null ? null : await MapAsync(u);
        }

        public async Task<string[]> GetRolesAsync(ApplicationUser user)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            if (u == null) return Array.Empty<string>();
            return (await _userManager.GetRolesAsync(u)).ToArray();
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            return u != null && await _userManager.CheckPasswordAsync(u, password);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            return u == null ? string.Empty : await _userManager.GeneratePasswordResetTokenAsync(u);
        }

        public async Task<(bool Succeeded, List<string> Errors)> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            if (u == null) return (false, new List<string> { "User not found." });
            var result = await _userManager.ResetPasswordAsync(u, token, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> AddToRoleAsync(ApplicationUser user, string roleName)
        {
            var u = await _userManager.FindByEmailAsync(user.Email!);
            if (u == null) return (false, new List<string> { "User not found." });
            var result = await _userManager.AddToRoleAsync(u, roleName);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            if (u == null) return (false, new List<string> { "User not found." });
            var result = await _userManager.RemoveFromRoleAsync(u, roleName);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        // ── new methods ──────────────────────────────────────────────────────────

        public async Task<(bool Succeeded, List<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            var u = await _userManager.FindByIdAsync(user.Id!);
            if (u == null) return (false, new List<string> { "User not found." });

            u.FirstName   = user.FirstName;
            u.LastName    = user.LastName;
            u.DateOfBirth = user.DateOfBirth;
            u.PhoneNumber = user.PhoneNumber;
            if (user.ProfileImageUrl      != null) u.ProfileImageUrl      = user.ProfileImageUrl;
            if (user.ProfileImagePublicId != null) u.ProfileImagePublicId = user.ProfileImagePublicId;

            var result = await _userManager.UpdateAsync(u);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<(bool Succeeded, List<string> Errors)> ChangePasswordAsync(
            string userId, string currentPassword, string newPassword)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return (false, new List<string> { "User not found." });
            var result = await _userManager.ChangePasswordAsync(u, currentPassword, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(
            int pageNumber, int pageSize, string? searchTerm)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lower = searchTerm.ToLower();
                query = query.Where(u =>
                    (u.Email     != null && u.Email.ToLower().Contains(lower))     ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lower)) ||
                    (u.LastName  != null && u.LastName.ToLower().Contains(lower)));
            }

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ApplicationUser>();
            foreach (var u in users)
                result.Add(await MapAsync(u));

            return result;
        }

        public async Task<int> GetUsersCountAsync(string? searchTerm)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lower = searchTerm.ToLower();
                query = query.Where(u =>
                    (u.Email     != null && u.Email.ToLower().Contains(lower))     ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lower)) ||
                    (u.LastName  != null && u.LastName.ToLower().Contains(lower)));
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByIdsAsync(IEnumerable<string> userIds)
        {
            var idList = userIds.ToList();
            var users  = await _userManager.Users
                .Where(u => idList.Contains(u.Id))
                .ToListAsync();

            var result = new List<ApplicationUser>();
            foreach (var u in users)
                result.Add(await MapAsync(u));

            return result;
        }

        public async Task<(bool Succeeded, List<string> Errors)> SetLockoutAsync(string userId, bool lockout)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return (false, new List<string> { "User not found." });

            await _userManager.SetLockoutEnabledAsync(u, true);
            var endDate = lockout ? DateTimeOffset.MaxValue : DateTimeOffset.UtcNow.AddSeconds(-1);
            var result  = await _userManager.SetLockoutEndDateAsync(u, endDate);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToList());
        }
    }
}
