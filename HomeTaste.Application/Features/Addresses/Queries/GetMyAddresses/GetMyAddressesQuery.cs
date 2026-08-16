using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Queries.GetMyAddresses
{
    public record GetMyAddressesQuery : IRequest<Result<IEnumerable<AddressResponse>>>;
}
