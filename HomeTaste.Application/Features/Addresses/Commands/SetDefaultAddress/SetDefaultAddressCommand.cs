using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.SetDefaultAddress
{
    public class SetDefaultAddressCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public SetDefaultAddressCommand(Guid id)
        {
            Id = id;
        }
    }
}
