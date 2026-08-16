using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Meals.Commands.UpdateMeal
{
    public record UpdateMealCommand(
        Guid Id,
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
