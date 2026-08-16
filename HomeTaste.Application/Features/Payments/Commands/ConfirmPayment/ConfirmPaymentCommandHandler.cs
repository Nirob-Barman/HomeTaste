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
using PaymentGatewayEntity = HomeTaste.Domain.Entities.Payment.PaymentGateway;

namespace HomeTaste.Application.Features.Payments.Commands.ConfirmPayment
{
    public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly ILoyaltyPointsService _loyaltyPointsService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IConfigEncryptor _encryptor;

        public ConfirmPaymentCommandHandler(
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

        public async Task<Result<PaymentTransactionResponse>> Handle(ConfirmPaymentCommand command, CancellationToken cancellationToken)
        {
            // Anonymous callers (gateway redirect callbacks) get Guid.Empty — skip ownership check for them
            Guid.TryParse(_userContextService.UserId, out var userId);

            var transaction = await _context.PaymentTransactions.FindAsync(new object?[] { command.TransactionId }, cancellationToken);
            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            if (transaction.Status != PaymentStatus.Pending)
                throw new BadRequestException(
                    transaction.Status == PaymentStatus.Success  ? "Payment is already confirmed."  :
                    transaction.Status == PaymentStatus.Refunded ? "Cannot confirm a refunded transaction." :
                                                                   "This transaction cannot be confirmed.");

            var order = await _context.Orders.FindAsync(new object?[] { transaction.OrderId }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Associated order not found.");

            if (userId != Guid.Empty && order.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            var gateway = await _context.PaymentGateways.FirstOrDefaultAsync(g => g.Slug == transaction.Gateway, cancellationToken);
            if (gateway == null)
                throw new NotFoundException("Payment gateway not found.");

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                throw new ServerErrorException($"No payment processor registered for gateway '{gateway.Slug}'.");

            var config = ParseConfig(gateway.Config);
            var verifyResult = await processor.VerifyAsync(config, transaction.TransactionRef, command.TransactionRef);

            if (!verifyResult.Success)
                throw new BadRequestException(verifyResult.Error ?? "Payment not yet completed. Please finish the payment.");

            await using var transactionScope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Let the processor dictate which ref gets stored (manual sets customer's TXN ID; card keeps PaymentIntentId)
                transaction.MarkSuccessful(verifyResult.TransactionRef, command.Notes);

                if (order.Status == OrderStatus.Pending)
                    order.UpdateStatus(OrderStatus.Confirmed);

                await _context.SaveChangesAsync(cancellationToken);
                await transactionScope.CommitAsync(cancellationToken);
            }
            catch
            {
                await transactionScope.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to confirm payment. Please try again.");
            }

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
