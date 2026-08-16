using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetTopMeals
{
    public class GetTopMealsQueryHandler : IRequestHandler<GetTopMealsQuery, Result<List<TopMealItem>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTopMealsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<TopMealItem>>> Handle(GetTopMealsQuery request, CancellationToken cancellationToken)
        {
            var result = await AnalyticsCalculations.GetTopMealsAsync(_context, request.Top, cancellationToken);
            return Result<List<TopMealItem>>.Ok(result, "Top meals retrieved successfully");
        }
    }
}
