using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<Result<RegisterResponse>>
    {
        public RegisterRequest Request { get; set; }

        public RegisterCommand(RegisterRequest request)
        {
            Request = request;
        }
    }
}
