using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Delivery
{
    public record UpdateDeliveryStatusRequest
    {
        public DeliveryStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
