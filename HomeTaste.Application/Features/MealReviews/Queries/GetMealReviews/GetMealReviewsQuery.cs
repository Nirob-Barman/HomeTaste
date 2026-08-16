using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMealReviews
{
    public class GetMealReviewsQuery : IRequest<Result<IEnumerable<ReviewResponse>>>
    {
        public Guid MealId { get; set; }

        public GetMealReviewsQuery(Guid mealId)
        {
            MealId = mealId;
        }
    }
}
