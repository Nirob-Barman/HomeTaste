using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public LoginRequest Request { get; set; }

        public LoginCommand(LoginRequest request)
        {
            Request = request;
        }
    }
}
