using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.DeleteAddress
{
    public class DeleteAddressCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteAddressCommand(Guid id)
        {
            Id = id;
        }
    }
}
