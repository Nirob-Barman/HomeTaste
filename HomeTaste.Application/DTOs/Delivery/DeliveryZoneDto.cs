namespace HomeTaste.Application.DTOs.Delivery
{
    public class CreateDeliveryZoneRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }

    public class UpdateDeliveryZoneRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }

    public class DeliveryZoneResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }

    public class ServiceabilityResponse
    {
        public bool IsServiceable { get; set; }
        public string? ZoneName { get; set; }
        public string? Message { get; set; }
    }
}
