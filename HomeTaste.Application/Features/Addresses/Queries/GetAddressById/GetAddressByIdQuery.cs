using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Queries.GetAddressById
{
    public record GetAddressByIdQuery(Guid Id) : IRequest<Result<AddressResponse>>;
}
