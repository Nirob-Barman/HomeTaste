using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetMyReviews
{
    public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, Result<IEnumerable<ReviewResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public GetMyReviewsQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<ReviewResponse>>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedException("Invalid User ID.");

            var reviews = await _context.MealReviews
                .Where(r => r.UserId == userId)
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
