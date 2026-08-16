using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>>
    {
        private readonly IUserManager _userManager;

        public GetAllUsersQueryHandler(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userManager.GetAllUsersAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var total = await _userManager.GetUsersCountAsync(request.SearchTerm);

            var items = new List<AdminUserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                items.Add(new AdminUserResponse
                {
                    Id = user.Id!,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    ProfileImageUrl = user.ProfileImageUrl,
                    IsLocked = user.IsLocked,
                    Roles = roles.ToList()
                });
            }

            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, total);
            meta.CurrentPageCount = items.Count;

            return Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<AdminUserResponse>> { Data = items, MetaData = meta },
                "Users retrieved successfully");
        }
    }
}
