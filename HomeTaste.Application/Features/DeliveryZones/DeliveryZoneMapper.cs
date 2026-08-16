namespace HomeTaste.Application.Features.DeliveryZones
{
    public static class DeliveryZoneMapper
    {
        public static DeliveryZoneResponse ToResponse(HomeTaste.Domain.Entities.Delivery.DeliveryZone zone) => new()
        {
            Id = zone.Id,
            Name = zone.Name,
            Description = zone.Description,
            IsActive = zone.IsActive,
            AllowedCities = zone.AllowedCities,
            AllowedPostalCodes = zone.AllowedPostalCodes
        };
    }
}
