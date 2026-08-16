using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Helpers.Email;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Email;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;
using OrderItemEntity = HomeTaste.Domain.Entities.Order.OrderItem;
using OrderItemCustomizationEntity = HomeTaste.Domain.Entities.Order.OrderItemCustomization;
using LoyaltyTransactionEntity = HomeTaste.Domain.Entities.Loyalty.LoyaltyTransaction;

namespace HomeTaste.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<OrderResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IDeliveryFeeService _deliveryFeeService;

        private const decimal TaxRate = 0.10m;
        private const int PointsRedemptionRate = 100; // 100 points = $1

        public PlaceOrderCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            INotificationService notificationService,
            IEmailService emailService,
            IDeliveryFeeService deliveryFeeService)
        {
            _context = context;
            _userContextService = userContextService;
            _notificationService = notificationService;
            _emailService = emailService;
            _deliveryFeeService = deliveryFeeService;
        }

        public async Task<Result<OrderResponse>> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                throw new UnauthorizedException("Invalid user.");

            var address = await _context.Addresses.FindAsync(new object?[] { command.AddressId }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            if (address.UserId != userId)
                throw new ForbiddenAccessException("Address does not belong to this user.");

            var activeZones = await _context.DeliveryZones.Where(z => z.IsActive).ToListAsync(cancellationToken);
            if (activeZones.Count > 0)
            {
                var city = address.City?.Trim().ToLowerInvariant() ?? "";
                var postal = address.PostalCode?.Trim().ToLowerInvariant() ?? "";

                var isServiceable = activeZones.Any(z =>
                    (!string.IsNullOrEmpty(city) && z.AllowedCities.Contains(city)) ||
                    (!string.IsNullOrEmpty(postal) && z.AllowedPostalCodes.Contains(postal)));

                if (!isServiceable)
                    throw new BadRequestException("Sorry, we don't deliver to this address yet.");
            }

            // Build order items
            var orderItems = new List<OrderItemEntity>();
            decimal subTotal = 0;

            foreach (var itemCommand in command.Items)
            {
                var meal = await _context.Meals.FindAsync(new object?[] { itemCommand.MealId }, cancellationToken);
                if (meal == null)
                    throw new NotFoundException($"Meal '{itemCommand.MealId}' not found.");

                if (itemCommand.Quantity <= 0)
                    throw new BadRequestException($"Quantity for '{meal.Name}' must be greater than zero.");

                decimal itemCustomizationTotal = 0;
                var customizations = new List<OrderItemCustomizationEntity>();

                if (itemCommand.CustomizationOptionIds != null && itemCommand.CustomizationOptionIds.Count > 0)
                {
                    foreach (var optionId in itemCommand.CustomizationOptionIds)
                    {
                        var option = await _context.MealCustomizationOptions.FindAsync(new object?[] { optionId }, cancellationToken);
                        if (option == null)
                            throw new NotFoundException($"Customization option '{optionId}' not found.");

                        if (option.MealId != itemCommand.MealId)
                            throw new BadRequestException($"Option '{option.Name}' does not belong to meal '{meal.Name}'.");

                        if (!option.IsAvailable)
                            throw new BadRequestException($"Option '{option.Name}' is not available.");

                        itemCustomizationTotal += option.AdditionalPrice;
                        customizations.Add(OrderItemCustomizationEntity.Create(option.Id, option.Name, option.AdditionalPrice));
                    }
                }

                var unitPrice = meal.Price + itemCustomizationTotal;
                var totalPrice = unitPrice * itemCommand.Quantity;
                subTotal += totalPrice;

                orderItems.Add(OrderItemEntity.Create(
                    meal.Id,
                    itemCommand.Quantity,
                    unitPrice,
                    totalPrice,
                    itemCommand.SpecialInstructions,
                    customizations));
            }

            // Loyalty points redemption
            int loyaltyPointsUsed = 0;
            decimal loyaltyDiscountAmount = 0;

            if (command.PointsToRedeem > 0)
            {
                var loyaltyAccount = await _context.LoyaltyAccounts
                    .FirstOrDefaultAsync(a => a.UserId == userId.ToString(), cancellationToken);

                if (loyaltyAccount == null || loyaltyAccount.CurrentPoints < command.PointsToRedeem)
                    throw new BadRequestException("Insufficient loyalty points.");

                loyaltyPointsUsed = command.PointsToRedeem;
                loyaltyDiscountAmount = Math.Round((decimal)loyaltyPointsUsed / PointsRedemptionRate, 2);
                loyaltyDiscountAmount = Math.Min(loyaltyDiscountAmount, subTotal);
            }

            // Coupon discount applied after loyalty
            decimal couponDiscountAmount = 0;
            Guid? couponId = null;

            if (!string.IsNullOrWhiteSpace(command.CouponCode))
            {
                var code = command.CouponCode.Trim().ToUpperInvariant();
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

                if (coupon == null)
                    throw new BadRequestException("Invalid coupon code.");

                if (!coupon.IsActive)
                    throw new BadRequestException("Coupon is inactive.");

                if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
                    throw new BadRequestException("Coupon has expired.");

                if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value)
                    throw new BadRequestException("Coupon usage limit reached.");

                var amountAfterLoyalty = subTotal - loyaltyDiscountAmount;
                if (coupon.MinOrderAmount.HasValue && amountAfterLoyalty < coupon.MinOrderAmount.Value)
                    throw new BadRequestException($"Minimum order of {coupon.MinOrderAmount:C} required for this coupon.");

                couponDiscountAmount = coupon.DiscountType == DiscountType.Percentage
                    ? amountAfterLoyalty * (coupon.DiscountValue / 100m)
                    : coupon.DiscountValue;

                if (coupon.MaxDiscountAmount.HasValue && couponDiscountAmount > coupon.MaxDiscountAmount.Value)
                    couponDiscountAmount = coupon.MaxDiscountAmount.Value;

                couponDiscountAmount = Math.Min(couponDiscountAmount, amountAfterLoyalty);
                couponDiscountAmount = Math.Round(couponDiscountAmount, 2);
                couponId = coupon.Id;
                coupon.IncrementUsageCount();
            }

            var deliveryFee = _deliveryFeeService.Calculate(subTotal);
            var totalDiscountAmount = loyaltyDiscountAmount + couponDiscountAmount;
            var taxableAmount = subTotal - totalDiscountAmount;
            var taxAmount = Math.Round(taxableAmount * TaxRate, 2);
            var totalAmount = taxableAmount + taxAmount + deliveryFee;

            var order = OrderEntity.Create(
                userId,
                command.AddressId,
                Math.Round(subTotal, 2),
                deliveryFee,
                Math.Round(couponDiscountAmount, 2),
                taxAmount,
                Math.Round(totalAmount, 2),
                couponId,
                command.Notes,
                loyaltyPointsUsed,
                loyaltyDiscountAmount,
                orderItems);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Orders.Add(order);

                // Deduct loyalty points if used
                if (loyaltyPointsUsed > 0)
                {
                    var loyaltyAccount = await _context.LoyaltyAccounts
                        .FirstOrDefaultAsync(a => a.UserId == userId.ToString(), cancellationToken);

                    if (loyaltyAccount != null)
                    {
                        loyaltyAccount.DeductPoints(loyaltyPointsUsed);

                        var loyaltyTx = LoyaltyTransactionEntity.Create(
                            loyaltyAccount.Id,
                            -loyaltyPointsUsed,
                            LoyaltyTransactionType.Redeemed,
                            order.Id,
                            $"Redeemed {loyaltyPointsUsed} points for order discount.");
                        _context.LoyaltyTransactions.Add(loyaltyTx);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to place order. Please try again.");
            }

            var response = await OrderMapper.BuildOrderResponseAsync(_context, order, cancellationToken);

            // Fire-and-forget after all DB work is done
            _ = _notificationService.CreateNotificationAsync(
                userId.ToString(),
                "Order Placed",
                $"Your order #{order.Id.ToString()[..8].ToUpperInvariant()} has been placed and is awaiting confirmation.",
                NotificationType.OrderStatus,
                order.Id,
                "Order");

            var userEmail = _userContextService.Email;
            if (!string.IsNullOrWhiteSpace(userEmail))
                _ = _emailService.SendEmailAsync(
                    $"Order Confirmed — #{order.Id.ToString()[..8].ToUpperInvariant()}",
                    OrderEmailTemplates.OrderConfirmation(response),
                    [userEmail]);

            return Result<OrderResponse>.Ok(response, "Order placed successfully.");
        }
    }
}
