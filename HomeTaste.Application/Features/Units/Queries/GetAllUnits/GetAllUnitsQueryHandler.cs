using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Units.Queries.GetAllUnits
{
    public class GetAllUnitsQueryHandler : IRequestHandler<GetAllUnitsQuery, Result<PaginatedResponse<IEnumerable<UnitResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUnitsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<UnitResponse>>>> Handle(GetAllUnitsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Units.Where(unit => unit.DeletedAt == null);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(unit =>
                    unit.Name!.Contains(request.SearchTerm) ||
                    unit.Abbreviation!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var unitResponses = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(unit => new UnitResponse
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    Abbreviation = unit.Abbreviation
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = unitResponses.Count;

            var response = new PaginatedResponse<IEnumerable<UnitResponse>>
            {
                Data = unitResponses,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<UnitResponse>>>.Ok(response, "Units retrieved successfully");
        }
    }
}
