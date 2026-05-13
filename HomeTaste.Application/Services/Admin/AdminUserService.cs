using HomeTaste.Application.DTOs.Admin;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Admin;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUserManager _userManager;

        public AdminUserService(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>> GetAllUsersAsync(
            int pageNumber = 1, int pageSize = 20, string? searchTerm = null)
        {
            var users = await _userManager.GetAllUsersAsync(pageNumber, pageSize, searchTerm);
            var total = await _userManager.GetUsersCountAsync(searchTerm);

            var items = new List<AdminUserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new AdminUserResponse
                {
                    Id              = user.Id!,
                    Email           = user.Email,
                    FirstName       = user.FirstName,
                    LastName        = user.LastName,
                    PhoneNumber     = user.PhoneNumber,
                    DateOfBirth     = user.DateOfBirth,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsLocked        = user.IsLocked,
                    Roles           = roles.ToList()
                });
            }

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, total);
            meta.CurrentPageCount = items.Count;

            return Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<AdminUserResponse>> { Data = items, MetaData = meta },
                "Users retrieved successfully");
        }

        public async Task<Result<AdminUserResponse>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<AdminUserResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<AdminUserResponse>.Ok(new AdminUserResponse
            {
                Id              = user.Id!,
                Email           = user.Email,
                FirstName       = user.FirstName,
                LastName        = user.LastName,
                PhoneNumber     = user.PhoneNumber,
                DateOfBirth     = user.DateOfBirth,
                ProfileImageUrl = user.ProfileImageUrl,
                IsLocked        = user.IsLocked,
                Roles           = roles.ToList()
            }, "User retrieved successfully");
        }

        public async Task<Result<bool>> BanUserAsync(string userId, BanUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found", "Not found", ResultType.NotFound);

            var (succeeded, errors) = await _userManager.SetLockoutAsync(userId, true);
            if (!succeeded)
                return Result<bool>.Fail(errors, "Ban failed", ResultType.Failure);

            return Result<bool>.Ok(true, "User banned successfully");
        }

        public async Task<Result<bool>> UnbanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Fail("User not found", "Not found", ResultType.NotFound);

            var (succeeded, errors) = await _userManager.SetLockoutAsync(userId, false);
            if (!succeeded)
                return Result<bool>.Fail(errors, "Unban failed", ResultType.Failure);

            return Result<bool>.Ok(true, "User unbanned successfully");
        }

        public async Task<Result<RoleAssignmentResponse>> AssignRoleAsync(AssignRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.RoleName))
                return Result<RoleAssignmentResponse>.Fail(
                    "UserId and RoleName are required", "Validation failed", ResultType.ValidationError);

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<RoleAssignmentResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            var (succeeded, errors) = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!succeeded)
                return Result<RoleAssignmentResponse>.Fail(errors, "Role assignment failed", ResultType.Failure);

            return Result<RoleAssignmentResponse>.Ok(
                new RoleAssignmentResponse { UserId = request.UserId, RoleName = request.RoleName },
                "Role assigned successfully");
        }

        public async Task<Result<RoleRemovalResponse>> RemoveRoleAsync(RemoveRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.RoleName))
                return Result<RoleRemovalResponse>.Fail(
                    "UserId and RoleName are required", "Validation failed", ResultType.ValidationError);

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Result<RoleRemovalResponse>.Fail("User not found", "Not found", ResultType.NotFound);

            var (succeeded, errors) = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            if (!succeeded)
                return Result<RoleRemovalResponse>.Fail(errors, "Role removal failed", ResultType.Failure);

            return Result<RoleRemovalResponse>.Ok(
                new RoleRemovalResponse { UserId = request.UserId, RoleName = request.RoleName },
                "Role removed successfully");
        }
    }
}
