using HomeTaste.Application.DTOs.Auth;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<Result<UserProfileResponse>>
    {
        public UpdateProfileRequest Request { get; set; }

        public UpdateProfileCommand(UpdateProfileRequest request)
        {
            Request = request;
        }
    }
}
