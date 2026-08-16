using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.UpdateAddress
{
    public record UpdateAddressCommand(
        Guid Id,
        string? Label,
        string? AddressLine1,
        string? AddressLine2,
        string? City,
        string? State,
        string? PostalCode,
        string? Country,
        double? Latitude,
        double? Longitude,
        bool IsDefault) : IRequest<Result<AddressResponse>>;
}
