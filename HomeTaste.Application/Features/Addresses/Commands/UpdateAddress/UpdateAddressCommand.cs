using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.UpdateAddress
{
    public class UpdateAddressCommand : IRequest<Result<AddressResponse>>
    {
        public Guid Id { get; set; }
        public AddressRequest Request { get; set; }

        public UpdateAddressCommand(Guid id, AddressRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
