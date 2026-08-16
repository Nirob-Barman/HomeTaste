using HomeTaste.Domain.Entities.Delivery;

namespace HomeTaste.Application.Features.Delivery
{
    public static class DeliveryMapper
    {
        public static DeliveryPersonnelResponse ToResponse(DeliveryPersonnel p) => new(
            p.Id,
            p.UserId,
            p.FullName,
            p.Phone,
            p.VehicleType,
            p.VehicleNumber,
            p.IsAvailable,
            p.CurrentLatitude,
            p.CurrentLongitude,
            p.Rating,
            p.TotalDeliveries,
            p.CreatedAt);

        public static DeliveryAssignmentResponse ToResponse(DeliveryAssignment a, string? personnelName) => new()
        {
            Id = a.Id,
            OrderId = a.OrderId,
            DeliveryPersonnelId = a.DeliveryPersonnelId,
            DeliveryPersonnelName = personnelName,
            Status = a.Status,
            StatusLabel = a.Status.ToString(),
            AssignedAt = a.AssignedAt,
            PickedUpAt = a.PickedUpAt,
            DeliveredAt = a.DeliveredAt,
            Notes = a.Notes
        };
    }
}
