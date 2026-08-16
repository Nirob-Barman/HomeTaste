using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.HardDeleteUnit
{
    public record HardDeleteUnitCommand(Guid Id) : IRequest<Result<bool>>;
}
