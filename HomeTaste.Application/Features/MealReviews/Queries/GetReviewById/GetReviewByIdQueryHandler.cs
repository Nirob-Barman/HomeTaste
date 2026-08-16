using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetReviewById
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Result<DetailedReviewResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserManager _userManager;

        public GetReviewByIdQueryHandler(IApplicationDbContext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<DetailedReviewResponse>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _context.MealReviews.FindAsync(new object?[] { request.ReviewId }, cancellationToken);
            if (review == null)
                throw new NotFoundException("Review not found");

            var user = await _userManager.FindByIdAsync(review.UserId.ToString());

            var reviewResponse = new DetailedReviewResponse
            {
                Id = review.Id,
                MealId = review.MealId,
                UserId = review.UserId,
                UserEmail = user?.Email ?? string.Empty,
                Rating = review.Rating,
                Feedback = review.Feedback,
                CreatedAt = review.CreatedAt
            };

            return Result<DetailedReviewResponse>.Ok(reviewResponse, "Review fetched successfully");
        }
    }
}
