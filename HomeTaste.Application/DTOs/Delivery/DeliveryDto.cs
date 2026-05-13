using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Delivery
{
    public class CreateDeliveryPersonnelRequest
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
    }

    public class UpdateDeliveryPersonnelRequest
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
    }

    public class UpdateLocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AssignDeliveryRequest
    {
        public Guid OrderId { get; set; }
        public Guid DeliveryPersonnelId { get; set; }
    }

    public class UpdateDeliveryStatusRequest
    {
        public DeliveryStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class DeliveryPersonnelResponse
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public bool IsAvailable { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class DeliveryAssignmentResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid DeliveryPersonnelId { get; set; }
        public string? DeliveryPersonnelName { get; set; }
        public DeliveryStatus Status { get; set; }
        public string? StatusLabel { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? Notes { get; set; }
    }
}
