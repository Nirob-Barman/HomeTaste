using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<Result<UserProfileResponse>>
    {
    }
}
