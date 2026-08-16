using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDailyRevenue
{
    public class GetDailyRevenueQueryHandler : IRequestHandler<GetDailyRevenueQuery, Result<List<DailyRevenuePoint>>>
    {
        private readonly IApplicationDbContext _context;

        public GetDailyRevenueQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<DailyRevenuePoint>>> Handle(GetDailyRevenueQuery request, CancellationToken cancellationToken)
        {
            var result = await AnalyticsCalculations.GetDailyRevenueAsync(_context, request.Days, cancellationToken);
            return Result<List<DailyRevenuePoint>>.Ok(result, "Daily revenue retrieved successfully");
        }
    }
}
