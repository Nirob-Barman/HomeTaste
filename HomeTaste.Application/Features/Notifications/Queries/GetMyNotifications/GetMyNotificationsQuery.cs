using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Queries.GetMyNotifications
{
    public record GetMyNotificationsQuery(int PageNumber = 1, int PageSize = 20)
        : IRequest<Result<PaginatedResponse<IEnumerable<NotificationResponse>>>>;
}
