namespace HomeTaste.Domain.Entities.Order
{
    public class OrderItemCustomization : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderItemId { get; set; }
        public Guid? CustomizationOptionId { get; set; }
        public string? Name { get; set; }
        public decimal AdditionalPrice { get; set; }

        public OrderItem? OrderItem { get; set; }
    }
}
