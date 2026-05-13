namespace HomeTaste.Domain.Entities.Address
{
    public class Address : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Label { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; }
    }
}
