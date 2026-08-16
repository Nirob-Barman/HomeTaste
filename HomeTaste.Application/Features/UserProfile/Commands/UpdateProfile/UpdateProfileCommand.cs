using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UpdateProfile
{
    public record UpdateProfileCommand(string? FirstName, string? LastName, DateTime? DateOfBirth, string? PhoneNumber)
        : IRequest<Result<UserProfileResponse>>;
}
