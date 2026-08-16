namespace HomeTaste.Application.Features.Delivery
{
    public record AssignDeliveryRequest
    {
        public Guid OrderId { get; set; }
        public Guid DeliveryPersonnelId { get; set; }
    }
}
