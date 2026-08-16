using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.DeleteNotification
{
    public record DeleteNotificationCommand(Guid Id) : IRequest<Result<bool>>;
}
