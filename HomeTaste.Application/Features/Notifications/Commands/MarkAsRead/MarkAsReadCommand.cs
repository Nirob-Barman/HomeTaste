using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid Id) : IRequest<Result<bool>>;
}
