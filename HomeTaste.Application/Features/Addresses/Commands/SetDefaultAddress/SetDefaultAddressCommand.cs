using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.SetDefaultAddress
{
    public record SetDefaultAddressCommand(Guid Id) : IRequest<Result<bool>>;
}
