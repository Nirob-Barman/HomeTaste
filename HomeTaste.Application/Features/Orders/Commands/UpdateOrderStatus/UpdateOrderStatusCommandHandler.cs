using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Email;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;

        public UpdateOrderStatusCommandHandler(
            IApplicationDbContext context,
            INotificationService notificationService,
            IEmailService emailService,
            IUserManager userManager)
        {
            _context = context;
            _notificationService = notificationService;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<Result<OrderResponse>> Handle(UpdateOrderStatusCommand command, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            var validationError = ValidateStatusTransition(order.Status, command.Status);
            if (validationError != null)
                throw new BadRequestException(validationError);

            order.UpdateStatus(command.Status, command.CancellationReason);

            await _context.SaveChangesAsync(cancellationToken);

            var response = await OrderMapper.BuildOrderResponseAsync(_context, order, cancellationToken);

            _ = _notificationService.CreateNotificationAsync(
                order.UserId.ToString(),
                "Order Update",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} is now {command.Status}.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            _ = SendStatusEmailAsync(order.UserId, order.Id, command.Status);

            return Result<OrderResponse>.Ok(response, "Order status updated successfully.");
        }

        private async Task SendStatusEmailAsync(Guid userId, Guid orderId, OrderStatus status)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (string.IsNullOrWhiteSpace(user?.Email)) return;
            await _emailService.SendEmailAsync(
                $"Order Update — #{orderId.ToString()[..8].ToUpperInvariant()}",
                OrderEmailTemplates.StatusChanged(orderId, status),
                [user.Email]);
        }

        private static string? ValidateStatusTransition(OrderStatus current, OrderStatus next)
        {
            var allowed = new Dictionary<OrderStatus, OrderStatus[]>
            {
                [OrderStatus.Pending]        = [OrderStatus.Confirmed, OrderStatus.Cancelled],
                [OrderStatus.Confirmed]      = [OrderStatus.Preparing, OrderStatus.Cancelled],
                [OrderStatus.Preparing]      = [OrderStatus.ReadyForPickup],
                [OrderStatus.ReadyForPickup] = [OrderStatus.OutForDelivery],
                [OrderStatus.OutForDelivery] = [OrderStatus.Delivered],
                [OrderStatus.Delivered]      = [],
                [OrderStatus.Cancelled]      = [],
                [OrderStatus.Refunded]       = [],
            };

            if (!allowed.TryGetValue(current, out var allowedNext) || !allowedNext.Contains(next))
                return $"Cannot transition from '{current}' to '{next}'.";

            return null;
        }
    }
}
