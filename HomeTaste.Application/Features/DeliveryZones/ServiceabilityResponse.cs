namespace HomeTaste.Application.Features.DeliveryZones
{
    public record ServiceabilityResponse
    {
        public bool IsServiceable { get; set; }
        public string? ZoneName { get; set; }
        public string? Message { get; set; }
    }
}
