using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.CreateMeal
{
    public record CreateMealCommand(
        string? Name,
        string? Description,
        decimal Price,
        Guid CategoryId,
        string? ImageUrl,
        bool IsAvailable,
        int? PreparationTime,
        decimal? DiscountPrice,
        int? Calories) : IRequest<Result<MealResponse>>;
}
