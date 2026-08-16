using HomeTaste.Domain.Enums;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryAssignment : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid DeliveryPersonnelId { get; private set; }
        public DeliveryStatus Status { get; private set; } = DeliveryStatus.Assigned;
        public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? PickedUpAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }
        public string? Notes { get; private set; }

        public OrderEntity? Order { get; set; }
        public DeliveryPersonnel? DeliveryPersonnel { get; set; }

        private DeliveryAssignment() { } // EF Core

        public static DeliveryAssignment Create(Guid orderId, Guid deliveryPersonnelId)
        {
            return new DeliveryAssignment
            {
                OrderId = orderId,
                DeliveryPersonnelId = deliveryPersonnelId,
                Status = DeliveryStatus.Assigned,
                AssignedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateStatus(DeliveryStatus status, string? notes)
        {
            Status = status;
            Notes = notes ?? Notes;
            UpdatedAt = DateTime.UtcNow;

            if (status == DeliveryStatus.PickedUp)
                PickedUpAt = DateTime.UtcNow;

            if (status == DeliveryStatus.Delivered)
                DeliveredAt = DateTime.UtcNow;
        }
    }
}
