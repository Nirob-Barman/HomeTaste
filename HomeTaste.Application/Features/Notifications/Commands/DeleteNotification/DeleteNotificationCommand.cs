using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteNotificationCommand(Guid id)
        {
            Id = id;
        }
    }
}
