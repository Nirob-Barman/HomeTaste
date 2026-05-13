using HomeTaste.Domain.Entities.MealManagement;

namespace HomeTaste.Domain.Entities.Order
{
    public class OrderItem : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid MealId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstructions { get; set; }

        public Order? Order { get; set; }
        public Meal? Meal { get; set; }
        public List<OrderItemCustomization>? Customizations { get; set; }
    }
}
