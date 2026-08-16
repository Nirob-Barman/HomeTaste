using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Queries.GetAllUsers
{
    public record GetAllUsersQuery(int PageNumber = 1, int PageSize = 20, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>>;
}
