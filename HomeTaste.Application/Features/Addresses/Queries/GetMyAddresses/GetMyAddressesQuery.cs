using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Queries.GetMyAddresses
{
    public class GetMyAddressesQuery : IRequest<Result<IEnumerable<AddressResponse>>>
    {
    }
}
