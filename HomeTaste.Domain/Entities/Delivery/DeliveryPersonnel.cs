namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryPersonnel : BaseEntity
    {
        public string? UserId { get; private set; }
        public string? FullName { get; private set; }
        public string? Phone { get; private set; }
        public string? VehicleType { get; private set; }
        public string? VehicleNumber { get; private set; }
        public bool IsAvailable { get; private set; } = true;
        public double? CurrentLatitude { get; private set; }
        public double? CurrentLongitude { get; private set; }
        public decimal Rating { get; private set; }
        public int TotalDeliveries { get; private set; }

        public List<DeliveryAssignment>? Assignments { get; set; }

        private DeliveryPersonnel() { } // EF Core

        public static DeliveryPersonnel Create(string? userId, string? fullName, string? phone, string? vehicleType, string? vehicleNumber)
        {
            return new DeliveryPersonnel
            {
                UserId = userId,
                FullName = fullName,
                Phone = phone,
                VehicleType = vehicleType,
                VehicleNumber = vehicleNumber,
                IsAvailable = true,
                Rating = 0,
                TotalDeliveries = 0,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(string? fullName, string? phone, string? vehicleType, string? vehicleNumber)
        {
            FullName = fullName;
            Phone = phone;
            VehicleType = vehicleType;
            VehicleNumber = vehicleNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleAvailability()
        {
            IsAvailable = !IsAvailable;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RecordCompletedDelivery()
        {
            TotalDeliveries++;
        }

        public void UpdateLocation(double latitude, double longitude)
        {
            CurrentLatitude = latitude;
            CurrentLongitude = longitude;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
