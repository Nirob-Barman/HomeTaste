using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<PaginatedResponse<IEnumerable<AdminUserResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
    }
}
