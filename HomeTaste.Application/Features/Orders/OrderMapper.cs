using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Domain.Entities.MealManagement;
using Microsoft.EntityFrameworkCore;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Application.Features.Orders
{
    public static class OrderMapper
    {
        public static async Task<IEnumerable<OrderResponse>> BuildOrderResponsesAsync(IApplicationDbContext context, IEnumerable<OrderEntity> orders, CancellationToken cancellationToken)
        {
            var responses = new List<OrderResponse>();
            foreach (var order in orders)
                responses.Add(await BuildOrderResponseAsync(context, order, cancellationToken));
            return responses;
        }

        public static async Task<OrderResponse> BuildOrderResponseAsync(IApplicationDbContext context, OrderEntity order, CancellationToken cancellationToken)
        {
            var items = new List<OrderItemResponse>();

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var meal = await context.Meals.FindAsync(new object?[] { item.MealId }, cancellationToken);

                    var itemCustomizations = await context.OrderItemCustomizations
                        .Where(c => c.OrderItemId == item.Id)
                        .ToListAsync(cancellationToken);

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

            var address = order.Address ?? await context.Addresses.FindAsync(new object?[] { order.AddressId }, cancellationToken);
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
                DeliveryFee = order.DeliveryFee,
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
