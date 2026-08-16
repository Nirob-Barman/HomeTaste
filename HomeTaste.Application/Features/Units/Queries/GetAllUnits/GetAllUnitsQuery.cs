using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetAllUnits
{
    public record GetAllUnitsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null)
        : IRequest<Result<PaginatedResponse<IEnumerable<UnitResponse>>>>;
}
