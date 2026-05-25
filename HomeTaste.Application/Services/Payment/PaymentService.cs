using System.Text.Json;
using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Validators.Payment;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Loyalty;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Payment;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;
using HomeTaste.Domain.Enums;
using PaymentGatewayEntity = HomeTaste.Domain.Entities.Payment.PaymentGateway;

namespace HomeTaste.Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly ILoyaltyService _loyaltyService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentProcessorFactory _processorFactory;
        private readonly IConfigEncryptor _encryptor;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            ILoyaltyService loyaltyService,
            INotificationService notificationService,
            IPaymentProcessorFactory processorFactory,
            IConfigEncryptor encryptor)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _loyaltyService = loyaltyService;
            _notificationService = notificationService;
            _processorFactory = processorFactory;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentTransactionResponse>> InitiatePaymentAsync(InitiatePaymentRequest request, string callbackBaseUrl)
        {
            var errors = InitiatePaymentRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<PaymentTransactionResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<PaymentTransactionResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);
            if (order == null)
                return Result<PaymentTransactionResponse>.Fail("Order not found.", "Not found", ResultType.NotFound);

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<PaymentTransactionResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded)
                return Result<PaymentTransactionResponse>.Fail($"Cannot initiate payment for a {order.Status} order.", "Bad request", ResultType.BadRequest);

            var existing = await _unitOfWork.Repository<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.OrderId == request.OrderId && t.Status == PaymentStatus.Success);
            if (existing != null)
                return Result<PaymentTransactionResponse>.Fail("This order has already been paid.", "Conflict", ResultType.Conflict);

            if (string.IsNullOrWhiteSpace(request.Gateway))
                return Result<PaymentTransactionResponse>.Fail("A payment gateway must be selected.", "Bad request", ResultType.BadRequest);

            var gatewaySlug = request.Gateway.Trim().ToLowerInvariant();
            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>()
                .FirstOrDefaultAsync(g => g.Slug == gatewaySlug && g.IsActive);
            if (gateway == null)
                return Result<PaymentTransactionResponse>.Fail(
                    $"No active '{gatewaySlug}' gateway configured. Please contact admin.",
                    "Service Unavailable", ResultType.Failure);

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                return Result<PaymentTransactionResponse>.Fail(
                    $"No payment processor registered for gateway '{gateway.Slug}'.",
                    "Service Unavailable", ResultType.Failure);

            // Generate txId upfront — redirect gateways embed it in the callback URL
            var tempTxId = Guid.NewGuid();
            var config = ParseConfig(gateway.Config);
            var successUrl = $"{callbackBaseUrl}/api/payment/callback/success?txId={tempTxId}&gateway={gateway.Slug}";
            var cancelUrl  = $"{callbackBaseUrl}/api/payment/callback/cancel?txId={tempTxId}&orderId={request.OrderId}";

            // Call processor first — no DB write yet
            var result = await processor.InitiateAsync(config, order.TotalAmount, request.OrderId, tempTxId, successUrl, cancelUrl);

            if (!result.Success)
                return Result<PaymentTransactionResponse>.Fail(
                    result.Error ?? "Payment initiation failed. Please contact admin.",
                    "Service Unavailable", ResultType.ServiceUnavailable);

            PaymentTransactionResponse response;

            if (result.RedirectUrl != null)
            {
                // Redirect flow — persist the record so the callback can look it up by txId
                await _unitOfWork.BeginTransaction();
                try
                {
                    var pendingTx = await _unitOfWork.Repository<PaymentTransaction>()
                        .FirstOrDefaultAsync(t => t.OrderId == request.OrderId && t.Status == PaymentStatus.Pending);
                    if (pendingTx != null)
                    {
                        pendingTx.Status = PaymentStatus.Failed;
                        pendingTx.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<PaymentTransaction>().Update(pendingTx);
                    }

                    var transaction = new PaymentTransaction
                    {
                        Id = tempTxId,
                        OrderId = request.OrderId,
                        Amount = order.TotalAmount,
                        Status = PaymentStatus.Pending,
                        Gateway = gateway.Slug,
                        TransactionRef = result.ProviderRef,
                        Notes = request.Notes,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    response = MapToResponse(transaction);
                    response.RedirectUrl = result.RedirectUrl;
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    return Result<PaymentTransactionResponse>.Fail(
                        "Failed to initiate payment. Please try again.", "Error", ResultType.Failure);
                }
            }
            else
            {
                // Direct flow (Stripe, manual) — no DB write; credentials returned only
                response = new PaymentTransactionResponse
                {
                    Id = tempTxId,
                    OrderId = request.OrderId,
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

            return Result<PaymentTransactionResponse>.Ok(response, message, ResultType.Created);
        }

        public async Task<Result<PaymentTransactionResponse>> ConfirmPaymentAsync(Guid transactionId, ConfirmPaymentRequest request)
        {
            var errors = ConfirmPaymentRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<PaymentTransactionResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            // Anonymous callers (gateway redirect callbacks) get Guid.Empty — skip ownership check for them
            Guid.TryParse(_userContextService.UserId, out var userId);

            var transaction = await _unitOfWork.Repository<PaymentTransaction>().GetByIdAsync(transactionId);
            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("Transaction not found.", "Not found", ResultType.NotFound);

            if (transaction.Status != PaymentStatus.Pending)
                return Result<PaymentTransactionResponse>.Fail(
                    transaction.Status == PaymentStatus.Success  ? "Payment is already confirmed."  :
                    transaction.Status == PaymentStatus.Refunded ? "Cannot confirm a refunded transaction." :
                                                                   "This transaction cannot be confirmed.",
                    "Bad request", ResultType.BadRequest);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(transaction.OrderId);
            if (order == null)
                return Result<PaymentTransactionResponse>.Fail("Associated order not found.", "Not found", ResultType.NotFound);

            if (userId != Guid.Empty && order.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<PaymentTransactionResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>()
                .FirstOrDefaultAsync(g => g.Slug == transaction.Gateway);
            if (gateway == null)
                return Result<PaymentTransactionResponse>.Fail("Payment gateway not found.", "Not found", ResultType.NotFound);

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                return Result<PaymentTransactionResponse>.Fail(
                    $"No payment processor registered for gateway '{gateway.Slug}'.",
                    "Service Unavailable", ResultType.Failure);

            var config = ParseConfig(gateway.Config);
            var verifyResult = await processor.VerifyAsync(config, transaction.TransactionRef, request.TransactionRef);

            if (!verifyResult.Success)
                return Result<PaymentTransactionResponse>.Fail(
                    verifyResult.Error ?? "Payment not yet completed. Please finish the payment.",
                    "Payment Required", ResultType.BadRequest);

            await _unitOfWork.BeginTransaction();
            try
            {
                // Let the processor dictate which ref gets stored (manual sets customer's TXN ID; card keeps PaymentIntentId)
                if (verifyResult.TransactionRef != null)
                    transaction.TransactionRef = verifyResult.TransactionRef;

                transaction.Status = PaymentStatus.Success;
                transaction.Notes = request.Notes ?? transaction.Notes;
                transaction.PaidAt = DateTime.UtcNow;
                transaction.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<PaymentTransaction>().Update(transaction);

                if (order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Confirmed;
                    order.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<OrderEntity>().Update(order);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<PaymentTransactionResponse>.Fail("Failed to confirm payment. Please try again.", "Error", ResultType.Failure);
            }

            _ = _loyaltyService.EarnPointsAsync(order.UserId.ToString(), order.Id, order.TotalAmount);
            _ = _notificationService.CreateNotificationAsync(
                order.UserId.ToString(),
                "Payment Confirmed",
                $"Payment of {transaction.Amount:C} confirmed via {transaction.Gateway}. Your order is now being prepared.",
                NotificationType.Payment,
                order.Id,
                "Order");

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Payment confirmed successfully.", ResultType.Success);
        }

        public async Task<Result<PaymentTransactionResponse>> ConfirmDirectPaymentAsync(ConfirmDirectPaymentRequest request)
        {
            var errors = ConfirmDirectPaymentRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<PaymentTransactionResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!Guid.TryParse(_userContextService.UserId, out var userId) || userId == Guid.Empty)
                return Result<PaymentTransactionResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);
            if (order == null)
                return Result<PaymentTransactionResponse>.Fail("Order not found.", "Not found", ResultType.NotFound);

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<PaymentTransactionResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded)
                return Result<PaymentTransactionResponse>.Fail($"Cannot confirm payment for a {order.Status} order.", "Bad request", ResultType.BadRequest);

            var existing = await _unitOfWork.Repository<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.OrderId == request.OrderId && t.Status == PaymentStatus.Success);
            if (existing != null)
                return Result<PaymentTransactionResponse>.Fail("This order has already been paid.", "Conflict", ResultType.Conflict);

            var gatewaySlug = request.Gateway!.Trim().ToLowerInvariant();
            var gateway = await _unitOfWork.Repository<PaymentGatewayEntity>()
                .FirstOrDefaultAsync(g => g.Slug == gatewaySlug && g.IsActive);
            if (gateway == null)
                return Result<PaymentTransactionResponse>.Fail(
                    $"No active '{gatewaySlug}' gateway configured. Please contact admin.",
                    "Service Unavailable", ResultType.Failure);

            var processor = _processorFactory.GetProcessor(gateway.Slug);
            if (processor == null)
                return Result<PaymentTransactionResponse>.Fail(
                    $"No payment processor registered for gateway '{gateway.Slug}'.",
                    "Service Unavailable", ResultType.Failure);

            var config = ParseConfig(gateway.Config);
            var verifyResult = await processor.VerifyAsync(config, null, request.TransactionRef);

            if (!verifyResult.Success)
                return Result<PaymentTransactionResponse>.Fail(
                    verifyResult.Error ?? "Payment verification failed. Please try again.",
                    "Payment Required", ResultType.BadRequest);

            await _unitOfWork.BeginTransaction();
            try
            {
                var transaction = new PaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    Amount = order.TotalAmount,
                    Status = PaymentStatus.Success,
                    Gateway = gateway.Slug,
                    TransactionRef = verifyResult.TransactionRef ?? request.TransactionRef,
                    Notes = request.Notes,
                    PaidAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);

                if (order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Confirmed;
                    order.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<OrderEntity>().Update(order);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                _ = _loyaltyService.EarnPointsAsync(order.UserId.ToString(), order.Id, order.TotalAmount);
                _ = _notificationService.CreateNotificationAsync(
                    order.UserId.ToString(),
                    "Payment Confirmed",
                    $"Payment of {transaction.Amount:C} confirmed via {transaction.Gateway}. Your order is now being prepared.",
                    NotificationType.Payment,
                    order.Id,
                    "Order");

                return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Payment confirmed successfully.", ResultType.Success);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<PaymentTransactionResponse>.Fail("Failed to confirm payment. Please try again.", "Error", ResultType.Failure);
            }
        }

        public async Task<Result<PaymentTransactionResponse>> CancelPendingPaymentAsync(Guid transactionId)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>().GetByIdAsync(transactionId);
            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("Transaction not found.", "Not found", ResultType.NotFound);

            if (transaction.Status != PaymentStatus.Pending)
                return Result<PaymentTransactionResponse>.Fail("Only pending transactions can be cancelled.", "Bad request", ResultType.BadRequest);

            // Skip ownership for anonymous callers (gateway cancel callbacks)
            Guid.TryParse(_userContextService.UserId, out var userId);
            if (userId != Guid.Empty)
            {
                var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(transaction.OrderId);
                if (order == null)
                    return Result<PaymentTransactionResponse>.Fail("Associated order not found.", "Not found", ResultType.NotFound);
                if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                    return Result<PaymentTransactionResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);
            }

            transaction.Status = PaymentStatus.Failed;
            transaction.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<PaymentTransaction>().Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Payment cancelled.", ResultType.Success);
        }

        public async Task<Result<PaymentTransactionResponse>> RefundPaymentAsync(Guid transactionId, RefundPaymentRequest request)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>().GetByIdAsync(transactionId);
            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("Transaction not found.", "Not found", ResultType.NotFound);

            if (transaction.Status != PaymentStatus.Success)
                return Result<PaymentTransactionResponse>.Fail("Only successful payments can be refunded.", "Bad request", ResultType.BadRequest);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(transaction.OrderId);
            if (order == null)
                return Result<PaymentTransactionResponse>.Fail("Associated order not found.", "Not found", ResultType.NotFound);

            await _unitOfWork.BeginTransaction();
            try
            {
                transaction.Status = PaymentStatus.Refunded;
                transaction.Notes = request.Notes ?? transaction.Notes;
                transaction.RefundedAt = DateTime.UtcNow;
                transaction.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<PaymentTransaction>().Update(transaction);

                order.Status = OrderStatus.Refunded;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<OrderEntity>().Update(order);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<PaymentTransactionResponse>.Fail("Failed to process refund. Please try again.", "Error", ResultType.Failure);
            }

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Refund processed successfully.", ResultType.Success);
        }

        public async Task<Result<PaymentTransactionResponse>> GetPaymentByOrderIdAsync(Guid orderId)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.OrderId == orderId);

            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("No payment found for this order.", "Not found", ResultType.NotFound);

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Payment retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<PaymentTransactionResponse>> GetPaymentByIdAsync(Guid id)
        {
            var transaction = await _unitOfWork.Repository<PaymentTransaction>().GetByIdAsync(id);
            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("Transaction not found.", "Not found", ResultType.NotFound);

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Transaction retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>> GetAllPaymentsAsync(int pageNumber = 1, int pageSize = 10, PaymentStatus? status = null)
        {
            var query = _unitOfWork.Repository<PaymentTransaction>().GetAllAsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            query = query.OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt);

            var totalCount = await _unitOfWork.Repository<PaymentTransaction>().CountAsync(query);
            var paged = _unitOfWork.Repository<PaymentTransaction>().PaginateAsQueryable(query, pageNumber, pageSize);
            var transactions = await _unitOfWork.Repository<PaymentTransaction>().ToEnumerableAsync(paged, t => MapToResponse(t));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<PaymentTransactionResponse>> { Data = transactions, MetaData = meta },
                "Transactions retrieved successfully.", ResultType.Success);
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private Dictionary<string, string> ParseConfig(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return new();
            string json;
            try { json = _encryptor.Decrypt(raw); }
            catch { json = raw; } // legacy plain-JSON fallback
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        }

        private static PaymentTransactionResponse MapToResponse(PaymentTransaction t) => new()
        {
            Id = t.Id,
            OrderId = t.OrderId,
            Amount = t.Amount,
            Status = t.Status,
            StatusLabel = t.Status.ToString(),
            Gateway = t.Gateway,
            TransactionRef = t.TransactionRef,
            Notes = t.Notes,
            PaidAt = t.PaidAt,
            RefundedAt = t.RefundedAt,
            CreatedAt = t.CreatedAt
        };
    }
}
