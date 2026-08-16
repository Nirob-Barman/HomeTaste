namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryZone : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        public List<string> AllowedCities { get; private set; } = [];
        public List<string> AllowedPostalCodes { get; private set; } = [];

        private DeliveryZone() { } // EF Core

        public static DeliveryZone Create(string name, string? description, bool isActive, List<string> allowedCities, List<string> allowedPostalCodes)
        {
            return new DeliveryZone
            {
                Name = name,
                Description = description,
                IsActive = isActive,
                AllowedCities = allowedCities,
                AllowedPostalCodes = allowedPostalCodes
            };
        }

        public void UpdateDetails(string name, string? description, bool isActive, List<string> allowedCities, List<string> allowedPostalCodes)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
            AllowedCities = allowedCities;
            AllowedPostalCodes = allowedPostalCodes;
        }
    }
}
