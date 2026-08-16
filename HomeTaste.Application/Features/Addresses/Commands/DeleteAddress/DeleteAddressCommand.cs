using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Addresses.Commands.DeleteAddress
{
    public record DeleteAddressCommand(Guid Id) : IRequest<Result<bool>>;
}
