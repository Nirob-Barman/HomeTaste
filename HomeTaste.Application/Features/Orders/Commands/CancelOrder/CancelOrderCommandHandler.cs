using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Email;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;

        public CancelOrderCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            INotificationService notificationService,
            IEmailService emailService,
            IUserManager userManager)
        {
            _context = context;
            _userContextService = userContextService;
            _notificationService = notificationService;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<Result<bool>> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var order = await _context.Orders.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                throw new BadRequestException("Order can only be cancelled when Pending or Confirmed.");

            order.UpdateStatus(OrderStatus.Cancelled, command.Reason);

            await _context.SaveChangesAsync(cancellationToken);

            _ = _notificationService.CreateNotificationAsync(
                order.UserId.ToString(),
                "Order Cancelled",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} has been cancelled.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            _ = SendCancelEmailAsync(order.UserId, order.Id, command.Reason);

            return Result<bool>.Ok(true, "Order cancelled successfully.");
        }

        private async Task SendCancelEmailAsync(Guid userId, Guid orderId, string? reason)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (string.IsNullOrWhiteSpace(user?.Email)) return;
            await _emailService.SendEmailAsync(
                $"Order Cancelled — #{orderId.ToString()[..8].ToUpperInvariant()}",
                OrderEmailTemplates.OrderCancelled(orderId, reason),
                [user.Email]);
        }
    }
}
