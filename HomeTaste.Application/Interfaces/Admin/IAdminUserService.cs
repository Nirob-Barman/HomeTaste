using HomeTaste.Application.DTOs.Admin;
using HomeTaste.Application.DTOs.UserProfile;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Admin
{
    public interface IAdminUserService
    {
        Task<Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null);
        Task<Result<AdminUserResponse>> GetUserByIdAsync(string userId);
        Task<Result<bool>> BanUserAsync(string userId, BanUserRequest request);
        Task<Result<bool>> UnbanUserAsync(string userId);
        Task<Result<RoleAssignmentResponse>> AssignRoleAsync(AssignRoleRequest request);
        Task<Result<RoleRemovalResponse>> RemoveRoleAsync(RemoveRoleRequest request);
    }
}
