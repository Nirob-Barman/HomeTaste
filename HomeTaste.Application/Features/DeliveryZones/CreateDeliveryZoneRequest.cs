namespace HomeTaste.Application.Features.DeliveryZones
{
    public record CreateDeliveryZoneRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }
}
