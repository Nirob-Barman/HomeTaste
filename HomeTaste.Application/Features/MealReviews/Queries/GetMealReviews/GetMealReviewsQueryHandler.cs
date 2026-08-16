using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMealReviews
{
    public class GetMealReviewsQueryHandler : IRequestHandler<GetMealReviewsQuery, Result<IEnumerable<ReviewResponse>>>
    {
        private readonly IApplicationDbContext _context;

        public GetMealReviewsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<ReviewResponse>>> Handle(GetMealReviewsQuery request, CancellationToken cancellationToken)
        {
            var meal = await _context.Meals.FindAsync(new object?[] { request.MealId }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found");

            var reviews = await _context.MealReviews
                .Where(r => r.MealId == request.MealId)
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    MealId = r.MealId,
                    MealName = r.Meal != null ? r.Meal.Name : null,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Feedback = r.Feedback,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<ReviewResponse>>.Ok(reviews, "Reviews fetched successfully");
        }
    }
}
