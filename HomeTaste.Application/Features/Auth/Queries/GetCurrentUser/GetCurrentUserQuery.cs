using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery : IRequest<Result<UserProfileResponse>>;
}
