namespace HomeTaste.Application.Features.Addresses
{
    public static class AddressMapper
    {
        public static AddressResponse ToResponse(HomeTaste.Domain.Entities.Address.Address address) => new()
        {
            Id = address.Id,
            UserId = address.UserId,
            Label = address.Label,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };
    }
}
