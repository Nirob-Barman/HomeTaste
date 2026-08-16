using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetUnitById
{
    public record GetUnitByIdQuery(Guid Id) : IRequest<Result<UnitResponse>>;
}
