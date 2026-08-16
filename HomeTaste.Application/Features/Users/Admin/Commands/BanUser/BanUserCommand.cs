using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Users.Admin.Commands.BanUser
{
    public class BanUserCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; }
        public BanUserRequest Request { get; set; }

        public BanUserCommand(string userId, BanUserRequest request)
        {
            UserId = userId;
            Request = request;
        }
    }
}
