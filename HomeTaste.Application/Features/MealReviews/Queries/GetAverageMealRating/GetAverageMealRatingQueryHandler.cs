using HomeTaste.Application.Common.Exceptions;
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
            var reviews = await _context.MealReviews
                .Where(r => r.MealId == request.MealId)
                .Select(r => r.Rating)
                .ToListAsync(cancellationToken);

            if (!reviews.Any())
                throw new NotFoundException("No reviews found for this meal.");

            var averageRating = (decimal)reviews.Average();

            return Result<decimal>.Ok(averageRating, "Average rating fetched successfully");
        }
    }
}
