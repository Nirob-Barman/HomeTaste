using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.SoftDeleteUnit
{
    public record SoftDeleteUnitCommand(Guid Id) : IRequest<Result<bool>>;
}
