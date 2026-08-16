using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<Result<string>>
    {
    }
}
