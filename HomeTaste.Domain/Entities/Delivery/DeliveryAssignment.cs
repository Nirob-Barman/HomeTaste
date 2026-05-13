using HomeTaste.Domain.Enums;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryAssignment : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid DeliveryPersonnelId { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? Notes { get; set; }

        public OrderEntity? Order { get; set; }
        public DeliveryPersonnel? DeliveryPersonnel { get; set; }
    }
}
