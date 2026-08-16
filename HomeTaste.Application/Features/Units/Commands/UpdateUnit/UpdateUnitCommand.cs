using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.UpdateUnit
{
    public record UpdateUnitCommand(Guid Id, string? Name, string? Abbreviation) : IRequest<Result<UnitResponse>>;
}
