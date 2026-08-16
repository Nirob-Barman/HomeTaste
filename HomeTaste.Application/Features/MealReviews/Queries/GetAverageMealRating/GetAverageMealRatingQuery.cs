using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetAverageMealRating
{
    public record GetAverageMealRatingQuery(Guid MealId) : IRequest<Result<decimal>>;
}
