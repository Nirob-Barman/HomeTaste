using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;
using PaymentGatewayEntity = HomeTaste.Domain.Entities.Payment.PaymentGateway;
using PaymentTransactionEntity = HomeTaste.Domain.Entities.Payment.PaymentTransaction;

namespace HomeTaste.Application.Features.Payments.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IConfigEncryptor _encryptor;

        public InitiatePaymentCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IPaymentProcessorFactory processorFactory,
            IConfigEncryptor encryptor)
        {
            _context = context;
            _userContextService = userContextService;
            _processorFactory = processorFactory;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(InitiatePaymentCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var order = await _context.Orders.FindAsync(new object?[] { command.OrderId }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new ForbiddenAccessException("Access denied.");

            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded)
                throw new BadRequestException($"Cannot initiate payment for a {order.Status} order.");

            var existing = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.OrderId == command.OrderId && t.Status == PaymentStatus.Success, cancellationToken);
            if (existing != null)
                throw new ConflictException("This order has already been paid.");

            if (string.IsNullOrWhiteSpace(command.Gateway))
                throw new BadRequestException("A payment gateway must be selected.");

            var gatewaySlug = command.Gateway.Trim().ToLowerInvariant();
            var gateway = await _context.PaymentGateways
                .FirstOrDefaultAsync(g => g.Slug == gatewaySlug && g.IsActive, cancellationToken);
            if (gateway == null)
                throw new ServerErrorException($"No active '{gatewaySlug}' gateway configured. Please contact admin.");

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                throw new ServerErrorException($"No payment processor registered for gateway '{gateway.Slug}'.");

            // Generate txId upfront — redirect gateways embed it in the callback URL
            var tempTxId = Guid.NewGuid();
            var config = ParseConfig(gateway.Config);
            var successUrl = $"{command.CallbackBaseUrl}/api/payment/callback/success?txId={tempTxId}&gateway={gateway.Slug}";
            var cancelUrl  = $"{command.CallbackBaseUrl}/api/payment/callback/cancel?txId={tempTxId}&orderId={command.OrderId}";

            // Call processor first — no DB write yet
            var result = await processor.InitiateAsync(config, order.TotalAmount, command.OrderId, tempTxId, successUrl, cancelUrl);

            if (!result.Success)
                throw new ServiceUnavailableException(result.Error ?? "Payment initiation failed. Please contact admin.");

            PaymentTransactionResponse response;

            if (result.RedirectUrl != null)
            {
                // Redirect flow — persist the record so the callback can look it up by txId
                await using var transactionScope = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var pendingTx = await _context.PaymentTransactions
                        .FirstOrDefaultAsync(t => t.OrderId == command.OrderId && t.Status == PaymentStatus.Pending, cancellationToken);
                    pendingTx?.MarkFailed();

                    var transaction = PaymentTransactionEntity.CreatePending(
                        tempTxId, command.OrderId, order.TotalAmount, gateway.Slug, result.ProviderRef, command.Notes);
                    _context.PaymentTransactions.Add(transaction);

                    await _context.SaveChangesAsync(cancellationToken);
                    await transactionScope.CommitAsync(cancellationToken);

                    response = PaymentMapper.ToResponse(transaction);
                    response.RedirectUrl = result.RedirectUrl;
                }
                catch
                {
                    await transactionScope.RollbackAsync(cancellationToken);
                    throw new ServerErrorException("Failed to initiate payment. Please try again.");
                }
            }
            else
            {
                // Direct flow (Stripe, manual) — no DB write; credentials returned only
                response = new PaymentTransactionResponse
                {
                    Id = tempTxId,
                    OrderId = command.OrderId,
                    Amount = order.TotalAmount,
                    Status = PaymentStatus.Pending,
                    StatusLabel = "Pending",
                    Gateway = gateway.Slug,
                    CreatedAt = DateTime.UtcNow,
                    ClientSecret = result.ClientSecret,
                    PublishableKey = result.PublishableKey,
                    MerchantNumber = result.MerchantNumber,
                };
            }

            var message = result.MerchantNumber != null
                ? "Payment initiated. Please complete the manual transfer."
                : result.RedirectUrl != null
                    ? "Payment initiated. Redirect to complete payment."
                    : "Payment initiated. Complete card payment.";

            return Result<PaymentTransactionResponse>.Ok(response, message);
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
