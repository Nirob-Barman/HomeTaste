using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MarkAsReadCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<bool>> Handle(MarkAsReadCommand command, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            var notification = await _context.Notifications.FindAsync(new object?[] { command.Id }, cancellationToken);

            if (notification == null)
                throw new NotFoundException("Notification not found.");

            if (notification.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            notification.MarkAsRead();
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Notification marked as read.");
        }
    }
}
