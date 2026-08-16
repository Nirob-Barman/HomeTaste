using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommand : IRequest<Result<bool>>
    {
    }
}
