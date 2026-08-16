using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.CategoryTypes.Queries.GetAllCategoryTypes
{
    public class GetAllCategoryTypesQueryHandler : IRequestHandler<GetAllCategoryTypesQuery, Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCategoryTypesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>> Handle(GetAllCategoryTypesQuery request, CancellationToken cancellationToken)
        {
            var categoryTypes = await _context.CategoryTypes
                .Select(ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                })
                .ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                categoryTypes = categoryTypes.Where(ct =>
                    ct.Name!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    ct.Description!.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var totalCount = categoryTypes.Count();

            var pagedCategoryTypes = categoryTypes
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);

            var currentPageCount = pagedCategoryTypes.Count();

            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<CategoryTypeResponse>>
            {
                Data = pagedCategoryTypes,
                MetaData = paginationMeta
            };

            if (!pagedCategoryTypes.Any())
            {
                throw new NotFoundException("No category types found");
            }

            return Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>.Ok(response, "Category types retrieved successfully");
        }
    }
}
