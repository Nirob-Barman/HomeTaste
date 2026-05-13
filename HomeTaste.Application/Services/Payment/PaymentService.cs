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

namespace HomeTaste.Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly ILoyaltyService _loyaltyService;
        private readonly INotificationService _notificationService;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            ILoyaltyService loyaltyService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _loyaltyService = loyaltyService;
            _notificationService = notificationService;
        }

        public async Task<Result<PaymentTransactionResponse>> InitiatePaymentAsync(InitiatePaymentRequest request)
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

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return Result<PaymentTransactionResponse>.Fail($"Cannot initiate payment for a {order.Status} order.", "Bad request", ResultType.BadRequest);

            var existing = await _unitOfWork.Repository<PaymentTransaction>()
                .FirstOrDefaultAsync(t => t.OrderId == request.OrderId && t.Status == PaymentStatus.Success);
            if (existing != null)
                return Result<PaymentTransactionResponse>.Fail("This order has already been paid.", "Conflict", ResultType.Conflict);

            var transaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending,
                Gateway = request.Gateway?.Trim().ToLowerInvariant() ?? "cash",
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentTransactionResponse>.Ok(MapToResponse(transaction), "Payment initiated. Awaiting confirmation.", ResultType.Created);
        }

        public async Task<Result<PaymentTransactionResponse>> ConfirmPaymentAsync(Guid transactionId, ConfirmPaymentRequest request)
        {
            var errors = ConfirmPaymentRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<PaymentTransactionResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var transaction = await _unitOfWork.Repository<PaymentTransaction>().GetByIdAsync(transactionId);
            if (transaction == null)
                return Result<PaymentTransactionResponse>.Fail("Transaction not found.", "Not found", ResultType.NotFound);

            if (transaction.Status == PaymentStatus.Success)
                return Result<PaymentTransactionResponse>.Fail("Payment is already confirmed.", "Conflict", ResultType.Conflict);

            if (transaction.Status == PaymentStatus.Refunded)
                return Result<PaymentTransactionResponse>.Fail("Cannot confirm a refunded transaction.", "Bad request", ResultType.BadRequest);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(transaction.OrderId);
            if (order == null)
                return Result<PaymentTransactionResponse>.Fail("Associated order not found.", "Not found", ResultType.NotFound);

            await _unitOfWork.BeginTransaction();
            try
            {
                transaction.Status = PaymentStatus.Success;
                transaction.TransactionRef = request.TransactionRef;
                transaction.Notes = request.Notes ?? transaction.Notes;
                transaction.PaidAt = DateTime.UtcNow;
                transaction.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<PaymentTransaction>().Update(transaction);

                // Advance order to Confirmed if still Pending
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

            // Earn loyalty points and notify — fire-and-forget; non-critical
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

            var totalCount = await _unitOfWork.Repository<PaymentTransaction>().CountAsync(query);

            var paged = _unitOfWork.Repository<PaymentTransaction>().PaginateAsQueryable(query, pageNumber, pageSize);
            var transactions = await _unitOfWork.Repository<PaymentTransaction>().ToEnumerableAsync(paged, t => MapToResponse(t));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<PaymentTransactionResponse>> { Data = transactions, MetaData = meta },
                "Transactions retrieved successfully.", ResultType.Success);
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
