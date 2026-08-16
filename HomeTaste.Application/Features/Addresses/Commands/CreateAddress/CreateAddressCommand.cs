using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.CreateAddress
{
    public class CreateAddressCommand : IRequest<Result<AddressResponse>>
    {
        public AddressRequest Request { get; set; }

        public CreateAddressCommand(AddressRequest request)
        {
            Request = request;
        }
    }
}
