namespace HomeTaste.Application.Features.Delivery
{
    public record DeliveryPersonnelResponse(
        Guid Id,
        string? UserId,
        string? FullName,
        string? Phone,
        string? VehicleType,
        string? VehicleNumber,
        bool IsAvailable,
        double? CurrentLatitude,
        double? CurrentLongitude,
        decimal Rating,
        int TotalDeliveries,
        DateTime? CreatedAt);
}
