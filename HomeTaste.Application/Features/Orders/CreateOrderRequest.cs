namespace HomeTaste.Application.Features.Orders
{
    public record CreateOrderRequest
    {
        public Guid AddressId { get; set; }
        public List<OrderItemRequest>? Items { get; set; }
        public string? CouponCode { get; set; }
        public int PointsToRedeem { get; set; }
        public string? Notes { get; set; }
    }

    public record OrderItemRequest
    {
        public Guid MealId { get; set; }
        public int Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<Guid>? CustomizationOptionIds { get; set; }
    }
}
