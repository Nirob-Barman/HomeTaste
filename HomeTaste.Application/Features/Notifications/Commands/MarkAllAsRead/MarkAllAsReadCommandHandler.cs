using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MarkAllAsReadCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(MarkAllAsReadCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var n in unread)
            {
                n.MarkAsRead();
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Ok(true, "All notifications marked as read.");
        }
    }
}
