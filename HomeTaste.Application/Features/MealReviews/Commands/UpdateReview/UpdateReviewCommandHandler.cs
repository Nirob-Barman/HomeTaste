using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.MealReviews.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public UpdateReviewCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<string>> Handle(UpdateReviewCommand command, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.UserId!;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedException("Invalid User ID.");

            var review = await _context.MealReviews.FindAsync(new object?[] { command.ReviewId }, cancellationToken);
            if (review == null)
                throw new NotFoundException("Review not found.");

            if (review.UserId != userId && !_userContextService.IsInRole("Admin"))
                throw new UnauthorizedException("You are not authorized to update this review.");

            review.UpdateFeedback(command.Request.Feedback, command.Request.Rating);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok("Review updated successfully", "Review updated successfully");
        }
    }
}
