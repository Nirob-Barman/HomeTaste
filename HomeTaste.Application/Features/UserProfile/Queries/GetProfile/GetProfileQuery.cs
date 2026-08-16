using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Queries.GetProfile
{
    public record GetProfileQuery : IRequest<Result<UserProfileResponse>>;
}
