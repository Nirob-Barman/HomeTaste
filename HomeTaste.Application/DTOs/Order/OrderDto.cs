using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Order
{
    public class CreateOrderRequest
    {
        public Guid AddressId { get; set; }
        public List<OrderItemRequest>? Items { get; set; }
        public string? CouponCode { get; set; }
        public int PointsToRedeem { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderItemRequest
    {
        public Guid MealId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<Guid>? CustomizationOptionIds { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public OrderStatus Status { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
    }

    public class DeliveryFeeResponse
    {
        public decimal Fee { get; set; }
        public bool IsFree => Fee == 0;
        public string Label => Fee == 0 ? "Free" : $"${Fee:F2}";
        public decimal FreeThreshold { get; set; } = 50m;
    }

    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public string? AddressSummary { get; set; }
        public OrderStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? CouponId { get; set; }
        public string? Notes { get; set; }
        public DateTime? EstimatedDeliveryAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }
        public int LoyaltyPointsUsed { get; set; }
        public decimal LoyaltyDiscountAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OrderItemResponse>? Items { get; set; }
    }

    public class OrderItemResponse
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public string? MealName { get; set; }
        public string? MealImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<OrderItemCustomizationResponse>? Customizations { get; set; }
    }

    public class OrderItemCustomizationResponse
    {
        public Guid Id { get; set; }
        public Guid? CustomizationOptionId { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }
    }
}
