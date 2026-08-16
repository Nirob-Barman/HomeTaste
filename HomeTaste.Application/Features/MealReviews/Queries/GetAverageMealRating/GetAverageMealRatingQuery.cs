using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetAverageMealRating
{
    public class GetAverageMealRatingQuery : IRequest<Result<decimal>>
    {
        public Guid MealId { get; set; }

        public GetAverageMealRatingQuery(Guid mealId)
        {
            MealId = mealId;
        }
    }
}
