namespace HomeTaste.Domain.Entities.Delivery
{
    public class DeliveryZone : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> AllowedCities { get; set; } = [];
        public List<string> AllowedPostalCodes { get; set; } = [];
    }
}
