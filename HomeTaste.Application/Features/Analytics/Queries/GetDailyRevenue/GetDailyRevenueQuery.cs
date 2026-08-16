using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDailyRevenue
{
    public class GetDailyRevenueQuery : IRequest<Result<List<DailyRevenuePoint>>>
    {
        public int Days { get; set; } = 30;
    }
}
