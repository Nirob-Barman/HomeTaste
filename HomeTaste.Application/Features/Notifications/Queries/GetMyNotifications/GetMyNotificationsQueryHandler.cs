using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Notifications.Queries.GetMyNotifications
{
    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PaginatedResponse<IEnumerable<NotificationResponse>>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyNotificationsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<NotificationResponse>>>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var page = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var notifications = page.Select(NotificationMapper.ToResponse).ToList();

            var meta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<NotificationResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<NotificationResponse>> { Data = notifications, MetaData = meta },
                "Notifications retrieved successfully.");
        }
    }
}
