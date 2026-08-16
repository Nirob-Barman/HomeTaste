using HomeTaste.Application.DTOs.Units;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using UnitEntity = HomeTaste.Domain.Entities.Units;
using MediatR;

namespace HomeTaste.Application.Features.Units.Queries.GetAllUnits
{
    public class GetAllUnitsQueryHandler : IRequestHandler<GetAllUnitsQuery, Result<PaginatedResponse<IEnumerable<UnitResponse>>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllUnitsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<UnitResponse>>>> Handle(GetAllUnitsQuery request, CancellationToken cancellationToken)
        {
            var unitResponses = await _unitOfWork.Repository<UnitEntity>().GetAllAsync(unit => unit.DeletedAt == null,
                unit => new UnitResponse
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    Abbreviation = unit.Abbreviation
                });

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                unitResponses = unitResponses.Where(unit =>
                    unit.Name!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    unit.Abbreviation!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = unitResponses.Count();

            var pagedUnits = unitResponses
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            var currentPageCount = pagedUnits.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<UnitResponse>>
            {
                Data = pagedUnits,
                MetaData = paginationMeta
            };

            if (!pagedUnits.Any())
            {
                return Result<PaginatedResponse<IEnumerable<UnitResponse>>>.Fail("No units found", "No units found", ResultType.NotFound);
            }

            return Result<PaginatedResponse<IEnumerable<UnitResponse>>>.Ok(response, "Units retrieved successfully", ResultType.Success);
        }
    }
}
