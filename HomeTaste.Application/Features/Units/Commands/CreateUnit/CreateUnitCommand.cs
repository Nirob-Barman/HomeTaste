using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Commands.CreateUnit
{
    public record CreateUnitCommand(string? Name, string? Abbreviation) : IRequest<Result<UnitResponse>>;
}
