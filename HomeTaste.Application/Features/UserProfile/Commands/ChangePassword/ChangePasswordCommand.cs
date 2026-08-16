using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.UserProfile.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result<bool>>
    {
        public ChangePasswordRequest Request { get; set; }

        public ChangePasswordCommand(ChangePasswordRequest request)
        {
            Request = request;
        }
    }
}
