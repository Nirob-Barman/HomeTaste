namespace HomeTaste.Domain.Entities.Order
{
    public class OrderItemCustomization : BaseEntity
    {
        public Guid OrderItemId { get; private set; }
        public Guid? CustomizationOptionId { get; private set; }
        public string? Name { get; private set; }
        public decimal AdditionalPrice { get; private set; }

        public OrderItem? OrderItem { get; set; }

        private OrderItemCustomization() { } // EF Core

        // Note: OrderItemId is intentionally never set here — the original never set it either,
        // relying entirely on EF Core's relationship fixup via OrderItem.Customizations.
        public static OrderItemCustomization Create(Guid? customizationOptionId, string? name, decimal additionalPrice)
        {
            return new OrderItemCustomization
            {
                Id = Guid.NewGuid(),
                CustomizationOptionId = customizationOptionId,
                Name = name,
                AdditionalPrice = additionalPrice,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
