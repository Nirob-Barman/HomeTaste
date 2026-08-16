using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Meals.Queries.GetAllMeals
{
    public class GetAllMealsQueryHandler : IRequestHandler<GetAllMealsQuery, Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllMealsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>> Handle(GetAllMealsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Meals.Include(m => m.MealCategory).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(meal =>
                    meal.Name!.Contains(request.SearchTerm) ||
                    meal.Description!.Contains(request.SearchTerm) ||
                    (meal.MealCategory != null && meal.MealCategory.Name!.Contains(request.SearchTerm))
                );
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(meal => meal.CategoryId == request.CategoryId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var meals = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(meal => new MealResponseWithMealCategory(
                    meal.Id,
                    meal.Name,
                    meal.Description,
                    meal.Price,
                    meal.ImageUrl,
                    meal.CategoryId,
                    meal.MealCategory != null ? meal.MealCategory.Name : null,
                    meal.IsAvailable,
                    meal.PreparationTime,
                    meal.DiscountPrice,
                    meal.Calories))
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = meals.Count;

            var response = new PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>
            {
                Data = meals,
                MetaData = paginationMeta
            };

            return Result<PaginatedResponse<IEnumerable<MealResponseWithMealCategory>>>.Ok(response, "Meals retrieved successfully");
        }
    }
}
