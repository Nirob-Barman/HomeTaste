using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Delivery
{
    public record DeliveryAssignmentResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid DeliveryPersonnelId { get; set; }
        public string? DeliveryPersonnelName { get; set; }
        public DeliveryStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? Notes { get; set; }
    }
}
