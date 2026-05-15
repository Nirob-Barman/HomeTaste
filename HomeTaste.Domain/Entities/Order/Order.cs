using HomeTaste.Domain.Enums;
using AddressEntity = HomeTaste.Domain.Entities.Address.Address;
using CouponEntity  = HomeTaste.Domain.Entities.Coupon.Coupon;

namespace HomeTaste.Domain.Entities.Order
{
    public class Order : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
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

        public AddressEntity? Address { get; set; }
        public CouponEntity?  Coupon  { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }
}
