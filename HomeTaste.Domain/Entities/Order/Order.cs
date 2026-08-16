using HomeTaste.Domain.Enums;
using AddressEntity = HomeTaste.Domain.Entities.Address.Address;
using CouponEntity  = HomeTaste.Domain.Entities.Coupon.Coupon;

namespace HomeTaste.Domain.Entities.Order
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid AddressId { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public decimal SubTotal { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public Guid? CouponId { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? EstimatedDeliveryAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? CancelledAt { get; private set; }
        public string? CancellationReason { get; private set; }
        public int LoyaltyPointsUsed { get; private set; }
        public decimal LoyaltyDiscountAmount { get; private set; }

        public AddressEntity? Address { get; set; }
        public CouponEntity?  Coupon  { get; set; }
        public List<OrderItem>? OrderItems { get; set; }

        private Order() { } // EF Core

        public static Order Create(
            Guid userId,
            Guid addressId,
            decimal subTotal,
            decimal deliveryFee,
            decimal discountAmount,
            decimal taxAmount,
            decimal totalAmount,
            Guid? couponId,
            string? notes,
            int loyaltyPointsUsed,
            decimal loyaltyDiscountAmount,
            List<OrderItem> orderItems)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(), // assigned upfront so order items can reference it before SaveChanges
                UserId = userId,
                AddressId = addressId,
                Status = OrderStatus.Pending,
                SubTotal = subTotal,
                DeliveryFee = deliveryFee,
                DiscountAmount = discountAmount,
                LoyaltyPointsUsed = loyaltyPointsUsed,
                LoyaltyDiscountAmount = loyaltyDiscountAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                CouponId = couponId,
                Notes = notes,
                EstimatedDeliveryAt = DateTime.UtcNow.AddMinutes(45),
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            foreach (var item in orderItems)
                item.AssignToOrder(order.Id);

            return order;
        }

        public void UpdateStatus(OrderStatus status, string? cancellationReason = null)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatus.Delivered)
                DeliveredAt = DateTime.UtcNow;

            if (status == OrderStatus.Cancelled)
            {
                CancelledAt = DateTime.UtcNow;
                CancellationReason = cancellationReason;
            }
        }
    }
}
