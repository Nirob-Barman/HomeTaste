using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public MarkAsReadCommand(Guid id)
        {
            Id = id;
        }
    }
}
