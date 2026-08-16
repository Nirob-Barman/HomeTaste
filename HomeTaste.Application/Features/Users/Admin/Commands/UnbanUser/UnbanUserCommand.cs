using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.UnbanUser
{
    public class UnbanUserCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }

        public UnbanUserCommand(string userId)
        {
            UserId = userId;
        }
    }
}
