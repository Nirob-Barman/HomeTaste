using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Queries.GetUnreadCount
{
    public record GetUnreadCountQuery : IRequest<Result<UnreadCountResponse>>;
}
