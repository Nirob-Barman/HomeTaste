using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Notifications.Queries.GetUnreadCount
{
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<UnreadCountResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetUnreadCountQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<UnreadCountResponse>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var count = await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

            return Result<UnreadCountResponse>.Ok(new UnreadCountResponse { Count = count }, "Unread count retrieved.");
        }
    }
}
