using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealIngredients.Queries.GetAllMealIngredients
{
    public class GetAllMealIngredientsQueryHandler : IRequestHandler<GetAllMealIngredientsQuery, Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllMealIngredientsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>> Handle(GetAllMealIngredientsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MealIngredients
                .Select(mi => new MealIngredientResponse
                {
                    Id = mi.Id,
                    MealId = mi.MealId,
                    MealName = mi.Meal!.Name,
                    IngredientId = mi.IngredientId,
                    IngredientName = mi.Ingredient!.Name,
                    Quantity = mi.Quantity,
                    UnitId = mi.UnitId,
                    UnitName = mi.Unit!.Name
                });

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(mi =>
                    mi.MealName!.Contains(request.SearchTerm) ||
                    mi.IngredientName!.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var mealIngredients = await query
                .OrderBy(mi => mi.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginationMeta = PaginationHelper.GetPaginationMetadata(request.PageNumber, request.PageSize, totalCount);
            paginationMeta.CurrentPageCount = mealIngredients.Count;

            var response = new PaginatedResponse<IEnumerable<MealIngredientResponse>>
            {
                MetaData = paginationMeta,
                Data = mealIngredients
            };

            return Result<PaginatedResponse<IEnumerable<MealIngredientResponse>>>.Ok(response, "Meal ingredients retrieved successfully");
        }
    }
}
