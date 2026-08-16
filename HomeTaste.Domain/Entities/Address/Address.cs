namespace HomeTaste.Domain.Entities.Address
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string? Label { get; private set; }
        public string? AddressLine1 { get; private set; }
        public string? AddressLine2 { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? PostalCode { get; private set; }
        public string? Country { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public bool IsDefault { get; private set; }

        private Address() { } // EF Core

        public static Address Create(
            Guid userId,
            string? label,
            string? addressLine1,
            string? addressLine2,
            string? city,
            string? state,
            string? postalCode,
            string? country,
            double? latitude,
            double? longitude,
            bool isDefault)
        {
            return new Address
            {
                UserId = userId,
                Label = label,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
                Latitude = latitude,
                Longitude = longitude,
                IsDefault = isDefault,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateDetails(
            string? label,
            string? addressLine1,
            string? addressLine2,
            string? city,
            string? state,
            string? postalCode,
            string? country,
            double? latitude,
            double? longitude,
            bool isDefault)
        {
            Label = label;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            IsDefault = isDefault;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearDefault()
        {
            IsDefault = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
