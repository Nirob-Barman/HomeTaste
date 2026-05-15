using HomeTaste.Application.DTOs.Order;
using HomeTaste.Application.Helpers.Email;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.Loyalty;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Application.Validators.Order;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Entities.Order;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;
        private readonly ILoyaltyService _loyaltyService;
        private readonly IEmailService _emailService;
        private readonly IUserManager _userManager;

        private const decimal TaxRate = 0.10m;
        private const int PointsRedemptionRate = 100; // 100 points = $1

        public OrderService(
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            INotificationService notificationService,
            ILoyaltyService loyaltyService,
            IEmailService emailService,
            IUserManager userManager)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _notificationService = notificationService;
            _loyaltyService = loyaltyService;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> GetMyOrdersAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<PaginatedResponse<IEnumerable<OrderResponse>>>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var query = _unitOfWork.Repository<Domain.Entities.Order.Order>()
                .WithIncludesAsQueryable(
                    o => o.Address!,
                    o => o.Coupon!,
                    o => o.OrderItems!);

            query = _unitOfWork.Repository<Domain.Entities.Order.Order>()
                .Where(query, o => o.UserId == userId);

            var totalCount = await _unitOfWork.Repository<Domain.Entities.Order.Order>().CountAsync(query);
            var paged = _unitOfWork.Repository<Domain.Entities.Order.Order>().PaginateAsQueryable(query, pageNumber, pageSize);
            var orders = await _unitOfWork.Repository<Domain.Entities.Order.Order>().ToListAsync(paged);

            var response = await BuildOrderResponsesAsync(orders);
            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            return Result<PaginatedResponse<IEnumerable<OrderResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<OrderResponse>> { Data = response, MetaData = meta },
                "Orders retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<OrderResponse>> GetOrderByIdAsync(Guid id)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<OrderResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var order = await _unitOfWork.Repository<Domain.Entities.Order.Order>().GetByIdAsync(id);
            if (order == null)
                return Result<OrderResponse>.Fail("Order not found.", "Not found", ResultType.NotFound);

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<OrderResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            var response = await BuildOrderResponseAsync(order);
            return Result<OrderResponse>.Ok(response, "Order retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<OrderResponse>>>> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 10, OrderStatus? status = null)
        {
            var query = _unitOfWork.Repository<Domain.Entities.Order.Order>()
                .WithIncludesAsQueryable(
                    o => o.Address!,
                    o => o.Coupon!,
                    o => o.OrderItems!);

            if (status.HasValue)
                query = _unitOfWork.Repository<Domain.Entities.Order.Order>()
                    .Where(query, o => o.Status == status.Value);

            var totalCount = await _unitOfWork.Repository<Domain.Entities.Order.Order>().CountAsync(query);
            var paged = _unitOfWork.Repository<Domain.Entities.Order.Order>().PaginateAsQueryable(query, pageNumber, pageSize);
            var orders = await _unitOfWork.Repository<Domain.Entities.Order.Order>().ToListAsync(paged);

            var response = await BuildOrderResponsesAsync(orders);
            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);

            return Result<PaginatedResponse<IEnumerable<OrderResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<OrderResponse>> { Data = response, MetaData = meta },
                "Orders retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<OrderResponse>> PlaceOrderAsync(CreateOrderRequest request)
        {
            var errors = CreateOrderRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<OrderResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<OrderResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(request.AddressId);
            if (address == null)
                return Result<OrderResponse>.Fail("Address not found.", "Not found", ResultType.NotFound);

            if (address.UserId != userId)
                return Result<OrderResponse>.Fail("Address does not belong to this user.", "Forbidden", ResultType.Forbidden);

            // Build order items
            var orderItems = new List<OrderItem>();
            decimal subTotal = 0;

            foreach (var itemRequest in request.Items)
            {
                var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(itemRequest.MealId);
                if (meal == null)
                    return Result<OrderResponse>.Fail($"Meal '{itemRequest.MealId}' not found.", "Not found", ResultType.NotFound);

                if (itemRequest.Quantity <= 0)
                    return Result<OrderResponse>.Fail($"Quantity for '{meal.Name}' must be greater than zero.", "Bad request", ResultType.BadRequest);

                decimal itemCustomizationTotal = 0;
                var customizations = new List<OrderItemCustomization>();

                if (itemRequest.CustomizationOptionIds != null && itemRequest.CustomizationOptionIds.Any())
                {
                    foreach (var optionId in itemRequest.CustomizationOptionIds)
                    {
                        var option = await _unitOfWork.Repository<MealCustomizationOption>().GetByIdAsync(optionId);
                        if (option == null)
                            return Result<OrderResponse>.Fail($"Customization option '{optionId}' not found.", "Not found", ResultType.NotFound);

                        if (option.MealId != itemRequest.MealId)
                            return Result<OrderResponse>.Fail($"Option '{option.Name}' does not belong to meal '{meal.Name}'.", "Bad request", ResultType.BadRequest);

                        if (!option.IsAvailable)
                            return Result<OrderResponse>.Fail($"Option '{option.Name}' is not available.", "Bad request", ResultType.BadRequest);

                        itemCustomizationTotal += option.AdditionalPrice;
                        customizations.Add(new OrderItemCustomization
                        {
                            Id = Guid.NewGuid(),
                            CustomizationOptionId = option.Id,
                            Name = option.Name,
                            AdditionalPrice = option.AdditionalPrice,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                var unitPrice = meal.Price + itemCustomizationTotal;
                var totalPrice = unitPrice * itemRequest.Quantity;
                subTotal += totalPrice;

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MealId = meal.Id,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice,
                    SpecialInstructions = itemRequest.SpecialInstructions,
                    Customizations = customizations,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // Loyalty points redemption
            int loyaltyPointsUsed = 0;
            decimal loyaltyDiscountAmount = 0;

            if (request.PointsToRedeem > 0)
            {
                var loyaltyAccount = await _unitOfWork.Repository<LoyaltyAccount>()
                    .FirstOrDefaultAsync(a => a.UserId == userId.ToString());

                if (loyaltyAccount == null || loyaltyAccount.CurrentPoints < request.PointsToRedeem)
                    return Result<OrderResponse>.Fail("Insufficient loyalty points.", "Bad request", ResultType.BadRequest);

                loyaltyPointsUsed = request.PointsToRedeem;
                loyaltyDiscountAmount = Math.Round((decimal)loyaltyPointsUsed / PointsRedemptionRate, 2);
                loyaltyDiscountAmount = Math.Min(loyaltyDiscountAmount, subTotal);
            }

            // Coupon discount applied after loyalty
            decimal couponDiscountAmount = 0;
            Guid? couponId = null;

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var code = request.CouponCode.Trim().ToUpperInvariant();
                var coupon = await _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>()
                    .FirstOrDefaultAsync(c => c.Code == code);

                if (coupon == null)
                    return Result<OrderResponse>.Fail("Invalid coupon code.", "Bad request", ResultType.BadRequest);

                if (!coupon.IsActive)
                    return Result<OrderResponse>.Fail("Coupon is inactive.", "Bad request", ResultType.BadRequest);

                if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
                    return Result<OrderResponse>.Fail("Coupon has expired.", "Bad request", ResultType.BadRequest);

                if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                    return Result<OrderResponse>.Fail("Coupon usage limit reached.", "Bad request", ResultType.BadRequest);

                var amountAfterLoyalty = subTotal - loyaltyDiscountAmount;
                if (coupon.MinOrderAmount.HasValue && amountAfterLoyalty < coupon.MinOrderAmount.Value)
                    return Result<OrderResponse>.Fail($"Minimum order of {coupon.MinOrderAmount:C} required for this coupon.", "Bad request", ResultType.BadRequest);

                couponDiscountAmount = coupon.DiscountType == DiscountType.Percentage
                    ? amountAfterLoyalty * (coupon.DiscountValue / 100m)
                    : coupon.DiscountValue;

                if (coupon.MaxDiscountAmount.HasValue && couponDiscountAmount > coupon.MaxDiscountAmount.Value)
                    couponDiscountAmount = coupon.MaxDiscountAmount.Value;

                couponDiscountAmount = Math.Min(couponDiscountAmount, amountAfterLoyalty);
                couponDiscountAmount = Math.Round(couponDiscountAmount, 2);
                couponId = coupon.Id;
                coupon.UsageCount++;
                _unitOfWork.Repository<Domain.Entities.Coupon.Coupon>().Update(coupon);
            }

            var totalDiscountAmount = loyaltyDiscountAmount + couponDiscountAmount;
            var taxableAmount = subTotal - totalDiscountAmount;
            var taxAmount = Math.Round(taxableAmount * TaxRate, 2);
            var totalAmount = taxableAmount + taxAmount;

            var order = new Domain.Entities.Order.Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AddressId = request.AddressId,
                Status = OrderStatus.Pending,
                SubTotal = Math.Round(subTotal, 2),
                DiscountAmount = Math.Round(couponDiscountAmount, 2),
                LoyaltyPointsUsed = loyaltyPointsUsed,
                LoyaltyDiscountAmount = loyaltyDiscountAmount,
                TaxAmount = taxAmount,
                TotalAmount = Math.Round(totalAmount, 2),
                CouponId = couponId,
                Notes = request.Notes,
                EstimatedDeliveryAt = DateTime.UtcNow.AddMinutes(45),
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            foreach (var item in orderItems)
                item.OrderId = order.Id;

            await _unitOfWork.BeginTransaction();
            try
            {
                await _unitOfWork.Repository<Domain.Entities.Order.Order>().AddAsync(order);

                // Deduct loyalty points if used
                if (loyaltyPointsUsed > 0)
                {
                    var loyaltyAccount = await _unitOfWork.Repository<LoyaltyAccount>()
                        .FirstOrDefaultAsync(a => a.UserId == userId.ToString());

                    if (loyaltyAccount != null)
                    {
                        loyaltyAccount.CurrentPoints -= loyaltyPointsUsed;
                        loyaltyAccount.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<LoyaltyAccount>().Update(loyaltyAccount);

                        var loyaltyTx = new LoyaltyTransaction
                        {
                            Id = Guid.NewGuid(),
                            LoyaltyAccountId = loyaltyAccount.Id,
                            Points = -loyaltyPointsUsed,
                            TransactionType = LoyaltyTransactionType.Redeemed,
                            ReferenceId = order.Id,
                            Description = $"Redeemed {loyaltyPointsUsed} points for order discount.",
                            CreatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.Repository<LoyaltyTransaction>().AddAsync(loyaltyTx);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<OrderResponse>.Fail("Failed to place order. Please try again.", "Error", ResultType.Failure);
            }

            // Notify customer (fire-and-forget — non-critical)
            _ = _notificationService.CreateNotificationAsync(
                userId.ToString(),
                "Order Placed",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} has been placed and is awaiting confirmation.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            var response = await BuildOrderResponseAsync(order);

            var userEmail = _userContextService.Email;
            if (!string.IsNullOrWhiteSpace(userEmail))
                _ = _emailService.SendEmailAsync(
                    $"Order Confirmed — #{order.Id.ToString()[..8].ToUpperInvariant()}",
                    OrderEmailTemplates.OrderConfirmation(response),
                    [userEmail]);

            return Result<OrderResponse>.Ok(response, "Order placed successfully.", ResultType.Created);
        }

        public async Task<Result<OrderResponse>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequest request)
        {
            var errors = UpdateOrderStatusRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<OrderResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var order = await _unitOfWork.Repository<Domain.Entities.Order.Order>().GetByIdAsync(id);
            if (order == null)
                return Result<OrderResponse>.Fail("Order not found.", "Not found", ResultType.NotFound);

            var validationError = ValidateStatusTransition(order.Status, request.Status);
            if (validationError != null)
                return Result<OrderResponse>.Fail(validationError, "Bad request", ResultType.BadRequest);

            order.Status = request.Status;
            order.UpdatedAt = DateTime.UtcNow;

            if (request.Status == OrderStatus.Delivered)
                order.DeliveredAt = DateTime.UtcNow;

            if (request.Status == OrderStatus.Cancelled)
            {
                order.CancelledAt = DateTime.UtcNow;
                order.CancellationReason = request.CancellationReason;
            }

            _unitOfWork.Repository<Domain.Entities.Order.Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            // Notify customer
            _ = _notificationService.CreateNotificationAsync(
                order.UserId.ToString(),
                "Order Update",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} is now {request.Status}.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            _ = SendStatusEmailAsync(order.UserId, order.Id, request.Status);

            var response = await BuildOrderResponseAsync(order);
            return Result<OrderResponse>.Ok(response, "Order status updated successfully.", ResultType.Success);
        }

        public async Task<Result<bool>> CancelOrderAsync(Guid id, CancelOrderRequest request)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<bool>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var order = await _unitOfWork.Repository<Domain.Entities.Order.Order>().GetByIdAsync(id);
            if (order == null)
                return Result<bool>.Fail("Order not found.", "Not found", ResultType.NotFound);

            if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<bool>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return Result<bool>.Fail("Order can only be cancelled when Pending or Confirmed.", "Bad request", ResultType.BadRequest);

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = request.Reason;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Domain.Entities.Order.Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            _ = _notificationService.CreateNotificationAsync(
                order.UserId.ToString(),
                "Order Cancelled",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} has been cancelled.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            _ = SendCancelEmailAsync(order.UserId, order.Id, request.Reason);

            return Result<bool>.Ok(true, "Order cancelled successfully.", ResultType.Success);
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

        private async Task SendCancelEmailAsync(Guid userId, Guid orderId, string? reason)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (string.IsNullOrWhiteSpace(user?.Email)) return;
            await _emailService.SendEmailAsync(
                $"Order Cancelled — #{orderId.ToString()[..8].ToUpperInvariant()}",
                OrderEmailTemplates.OrderCancelled(orderId, reason),
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

        private async Task<IEnumerable<OrderResponse>> BuildOrderResponsesAsync(IEnumerable<Domain.Entities.Order.Order> orders)
        {
            var responses = new List<OrderResponse>();
            foreach (var order in orders)
                responses.Add(await BuildOrderResponseAsync(order));
            return responses;
        }

        private async Task<OrderResponse> BuildOrderResponseAsync(Domain.Entities.Order.Order order)
        {
            var items = new List<OrderItemResponse>();

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(item.MealId);

                    var itemCustomizations = await _unitOfWork.Repository<OrderItemCustomization>()
                        .Where(c => c.OrderItemId == item.Id);

                    items.Add(new OrderItemResponse
                    {
                        Id = item.Id,
                        MealId = item.MealId,
                        MealName = meal?.Name,
                        MealImageUrl = meal?.ImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        SpecialInstructions = item.SpecialInstructions,
                        Customizations = itemCustomizations.Select(c => new OrderItemCustomizationResponse
                        {
                            Id = c.Id,
                            CustomizationOptionId = c.CustomizationOptionId,
                            Name = c.Name,
                            AdditionalPrice = c.AdditionalPrice
                        }).ToList()
                    });
                }
            }

            var address = order.Address ?? await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(order.AddressId);
            var addressSummary = address != null
                ? $"{address.AddressLine1}, {address.City}, {address.Country}"
                : null;

            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                AddressId = order.AddressId,
                AddressSummary = addressSummary,
                Status = order.Status,
                StatusLabel = order.Status.ToString(),
                SubTotal = order.SubTotal,
                DiscountAmount = order.DiscountAmount,
                LoyaltyPointsUsed = order.LoyaltyPointsUsed,
                LoyaltyDiscountAmount = order.LoyaltyDiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                CouponId = order.CouponId,
                Notes = order.Notes,
                EstimatedDeliveryAt = order.EstimatedDeliveryAt,
                DeliveredAt = order.DeliveredAt,
                CancelledAt = order.CancelledAt,
                CancellationReason = order.CancellationReason,
                CreatedAt = order.CreatedAt,
                Items = items
            };
        }
    }
}
