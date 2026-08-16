using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteReviewCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(DeleteReviewCommand command, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedException("Invalid User ID.");

            var review = await _context.MealReviews.FindAsync(new object?[] { command.ReviewId }, cancellationToken);
            if (review == null)
                throw new NotFoundException("Review not found.");

            if (review.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new UnauthorizedException("You are not authorized to delete this review.");

            _context.MealReviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok("Review deleted successfully", "Review deleted successfully");
        }
    }
}
