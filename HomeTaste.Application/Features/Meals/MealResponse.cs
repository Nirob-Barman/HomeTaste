namespace HomeTaste.Application.Features.Meals
{
    public record MealResponse(
        Guid Id,
        string? Name,
        string? Description,
        decimal Price,
        string? ImageUrl,
        Guid CategoryId,
        bool IsAvailable,
        int? PreparationTime,
        decimal? DiscountPrice,
        int? Calories);

    public record MealResponseWithMealCategory(
        Guid Id,
        string? Name,
        string? Description,
        decimal Price,
        string? ImageUrl,
        Guid CategoryId,
        string? CategoryName,
        bool IsAvailable,
        int? PreparationTime,
        decimal? DiscountPrice,
        int? Calories);
}
