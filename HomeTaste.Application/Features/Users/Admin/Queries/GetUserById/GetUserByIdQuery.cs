using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Queries.GetUserById
{
    public record GetUserByIdQuery(string UserId) : IRequest<Result<AdminUserResponse>>;
}
