using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetAllUnits
{
    public class GetAllUnitsQuery : IRequest<Result<PaginatedResponse<IEnumerable<UnitResponse>>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = null!;
    }
}
