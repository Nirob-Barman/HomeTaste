using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Queries.GetAddressById
{
    public class GetAddressByIdQuery : IRequest<Result<AddressResponse>>
    {
        public Guid Id { get; set; }

        public GetAddressByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
