using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealCategories.Queries.GetMealCategoryById
{
    public class GetMealCategoryByIdQueryHandler : IRequestHandler<GetMealCategoryByIdQuery, Result<MealCategoryResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetMealCategoryByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealCategoryResponse>> Handle(GetMealCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var mealCategory = await _context.MealCategories.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (mealCategory == null)
                throw new NotFoundException("Meal category not found");

            var response = new MealCategoryResponse
            {
                Id = mealCategory.Id,
                Name = mealCategory.Name,
                Description = mealCategory.Description,
                ImageUrl = mealCategory.ImageUrl
            };

            return Result<MealCategoryResponse>.Ok(response, "Meal category retrieved successfully");
        }
    }
}
