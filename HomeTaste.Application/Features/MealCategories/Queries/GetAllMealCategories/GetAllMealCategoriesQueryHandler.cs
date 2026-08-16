using HomeTaste.Application.DTOs.MealManagement;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetAllMealCategories
{
    public class GetAllMealCategoriesQueryHandler : IRequestHandler<GetAllMealCategoriesQuery, Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>>
    {
        private static readonly List<string> ValidSortColumns = new() { "Id", "Name", "CreatedAt" };

        private readonly IApplicationDbContext _context;

        public GetAllMealCategoriesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>> Handle(GetAllMealCategoriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MealCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(mealCategory =>
                    mealCategory.Name!.Contains(request.SearchTerm) ||
                    mealCategory.Description!.Contains(request.SearchTerm)
                );
            }

            if (!ValidSortColumns.Contains(request.SortBy))
            {
                return Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>.Fail("Invalid sort column", "Invalid sort column.", ResultType.Failure);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var descending = string.Equals(request.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            query = request.SortBy switch
            {
                "Name" => descending ? query.OrderByDescending(mc => mc.Name) : query.OrderBy(mc => mc.Name),
                "CreatedAt" => descending ? query.OrderByDescending(mc => mc.CreatedAt) : query.OrderBy(mc => mc.CreatedAt),
                _ => descending ? query.OrderByDescending(mc => mc.Id) : query.OrderBy(mc => mc.Id),
            };

            var mealCategories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(mealCategory => new MealCategoryResponse
                {
                    Id = mealCategory.Id,
                    Name = mealCategory.Name,
                    Description = mealCategory.Description,
                    ImageUrl = mealCategory.ImageUrl
                })
                .ToListAsync(cancellationToken);

            var currentPageCount = mealCategories.Count;
            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = currentPageCount;

            var response = new PaginatedResponse<IEnumerable<MealCategoryResponse>>
            {
                Data = mealCategories,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<MealCategoryResponse>>>.Ok(response, "Meal categories retrieved successfully", ResultType.Success);
        }
    }
}
