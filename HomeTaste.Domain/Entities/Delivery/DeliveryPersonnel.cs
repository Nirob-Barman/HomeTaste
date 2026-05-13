namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryPersonnel : BaseEntity
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public bool IsAvailable { get; set; } = true;
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }

        public List<DeliveryAssignment>? Assignments { get; set; }
    }
}
