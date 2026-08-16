using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealIngredients.Queries.GetMealIngredientById
{
    public class GetMealIngredientByIdQueryHandler : IRequestHandler<GetMealIngredientByIdQuery, Result<MealIngredientResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetMealIngredientByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MealIngredientResponse>> Handle(GetMealIngredientByIdQuery request, CancellationToken cancellationToken)
        {
            var mealIngredient = await _context.MealIngredients.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (mealIngredient == null)
                throw new NotFoundException("Meal Ingredient not found");

            var response = new MealIngredientResponse
            {
                Id = mealIngredient.Id,
                MealId = mealIngredient.MealId,
                IngredientId = mealIngredient.IngredientId,
                Quantity = mealIngredient.Quantity,
                UnitId = mealIngredient.UnitId
            };

            return Result<MealIngredientResponse>.Ok(response, "Meal Ingredient retrieved successfully");
        }
    }
}
