namespace HomeTaste.Application.Features.Delivery
{
    public record CreateDeliveryPersonnelRequest
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
    }
}
