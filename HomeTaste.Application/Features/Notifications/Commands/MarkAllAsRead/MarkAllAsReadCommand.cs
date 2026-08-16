using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public record MarkAllAsReadCommand : IRequest<Result<bool>>;
}
