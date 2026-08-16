namespace HomeTaste.Application.Features.DeliveryZones
{
    public record DeliveryZoneResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }
}
