using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.TimeManagement;
using HomeTaste.Application.Wrappers;
using MediatR;
using ReviewEntity = HomeTaste.Domain.Entities.MealManagement.MealReview;

namespace HomeTaste.Application.Features.MealReviews.Commands.SubmitReview
{
    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<string>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTimeService _dateTimeService;

        public SubmitReviewCommandHandler(IApplicationDbContext context, IDateTimeService dateTimeService)
        {
            _context = context;
            _dateTimeService = dateTimeService;
        }

        public async Task<Result<string>> Handle(SubmitReviewCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var meal = await _context.Meals.FindAsync(new object?[] { request.MealId }, cancellationToken);
            if (meal == null)
                throw new NotFoundException("Meal not found");

            var review = ReviewEntity.Create(
                request.MealId,
                request.UserId,
                request.Rating,
                request.Feedback,
                _dateTimeService.GetLocalNow());

            _context.MealReviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<string>.Ok("Review submitted successfully", "Review submitted successfully");
        }
    }
}
