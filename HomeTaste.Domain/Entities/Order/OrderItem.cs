using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Domain.Entities.Order
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid MealId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string? SpecialInstructions { get; private set; }

        public Order? Order { get; set; }
        public Meal? Meal { get; set; }
        public List<OrderItemCustomization>? Customizations { get; private set; }

        private OrderItem() { } // EF Core

        public static OrderItem Create(
            Guid mealId,
            int quantity,
            decimal unitPrice,
            decimal totalPrice,
            string? specialInstructions,
            List<OrderItemCustomization> customizations)
        {
            return new OrderItem
            {
                Id = Guid.NewGuid(), // assigned upfront to match the original's explicit pre-generation
                MealId = mealId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice,
                SpecialInstructions = specialInstructions,
                Customizations = customizations,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void AssignToOrder(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
