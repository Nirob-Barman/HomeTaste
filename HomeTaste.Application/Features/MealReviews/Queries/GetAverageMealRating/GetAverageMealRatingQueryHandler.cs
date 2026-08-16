using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.MealReviews.Queries.GetAverageMealRating
{
    public class GetAverageMealRatingQueryHandler : IRequestHandler<GetAverageMealRatingQuery, Result<decimal>>
    {
        private readonly IApplicationDbContext _context;

        public GetAverageMealRatingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<decimal>> Handle(GetAverageMealRatingQuery request, CancellationToken cancellationToken)
        {
            var query = _context.MealReviews.Where(r => r.MealId == request.MealId);

            var hasReviews = await query.AnyAsync(cancellationToken);
            if (!hasReviews)
                return Result<decimal>.Ok(0m, "No reviews yet for this meal.");

            var averageRating = await query.AverageAsync(r => (decimal)r.Rating, cancellationToken);

            return Result<decimal>.Ok(averageRating, "Average rating fetched successfully");
        }
    }
}
