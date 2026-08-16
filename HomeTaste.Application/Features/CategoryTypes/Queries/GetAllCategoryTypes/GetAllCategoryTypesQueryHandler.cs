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
            var query = _context.CategoryTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(ct =>
                    ct.Name!.Contains(request.SearchTerm) ||
                    ct.Description!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var categoryTypes = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(ct => new CategoryTypeResponse
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description
                })
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = categoryTypes.Count;

            var response = new PaginatedResponse<IEnumerable<CategoryTypeResponse>>
            {
                Data = categoryTypes,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<CategoryTypeResponse>>>.Ok(response, "Category types retrieved successfully");
        }
    }
}
