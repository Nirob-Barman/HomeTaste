using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Loyalty;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentTransactionEntity = HomeTaste.Domain.Entities.Payment.PaymentTransaction;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmDirectPayment
{
    public class ConfirmDirectPaymentCommandHandler : IRequestHandler<ConfirmDirectPaymentCommand, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILoyaltyPointsService _loyaltyPointsService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IConfigEncryptor _encryptor;

        public ConfirmDirectPaymentCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            ILoyaltyPointsService loyaltyPointsService,
            INotificationService notificationService,
            IPaymentProcessorFactory processorFactory,
            IConfigEncryptor encryptor)
        {
            _context = context;
            _userContextService = userContextService;
            _loyaltyPointsService = loyaltyPointsService;
            _notificationService = notificationService;
            _processorFactory = processorFactory;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(ConfirmDirectPaymentCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId) || userId == Guid.Empty)
                throw new UnauthorizedException("Invalid user.");

            var order = await _context.Orders.FindAsync(new object?[] { command.OrderId }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded)
                throw new BadRequestException($"Cannot confirm payment for a {order.Status} order.");

            var existing = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.OrderId == command.OrderId && t.Status == PaymentStatus.Success, cancellationToken);
            if (existing != null)
                throw new ConflictException("This order has already been paid.");

            var gatewaySlug = command.Gateway!.Trim().ToLowerInvariant();
            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Slug == gatewaySlug && g.IsActive, cancellationToken);
            if (gateway == null)
                throw new ServerErrorException($"No active '{gatewaySlug}' gateway configured. Please contact admin.");

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                throw new ServerErrorException($"No payment processor registered for gateway '{gateway.Slug}'.");

            var config = ParseConfig(gateway.Config);
            var verifyResult = await processor.VerifyAsync(config, null, command.TransactionRef);

            if (!verifyResult.Success)
                throw new BadRequestException(verifyResult.Error ?? "Payment verification failed. Please try again.");

            await using var transactionScope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var transaction = PaymentTransactionEntity.CreateSuccessful(
                    command.OrderId, order.TotalAmount, gateway.Slug,
                    verifyResult.TransactionRef ?? command.TransactionRef, command.Notes);
                _context.PaymentTransactions.Add(transaction);

                if (order.Status == OrderStatus.Pending)
                    order.UpdateStatus(OrderStatus.Confirmed);

                await _context.SaveChangesAsync(cancellationToken);
                await transactionScope.CommitAsync(cancellationToken);

                _ = _loyaltyPointsService.EarnPointsAsync(order.UserId.ToString(), order.Id, order.TotalAmount);
                _ = _notificationService.CreateNotificationAsync(
                    order.UserId.ToString(),
                    "Payment Confirmed",
                    $"Payment of {transaction.Amount:C} confirmed via {transaction.Gateway}. Your order is now being prepared.",
                    NotificationType.Payment,
                    order.Id,
                    "Order");

                return Result<PaymentTransactionResponse>.Ok(PaymentMapper.ToResponse(transaction), "Payment confirmed successfully.");
            }
            catch
            {
                await transactionScope.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to confirm payment. Please try again.");
            }
        }

        private Dictionary<string, string> ParseConfig(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return new();
            string json;
            try { json = _encryptor.Decrypt(raw); }
            catch { json = raw; } // legacy plain-JSON fallback
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }
    }
}
