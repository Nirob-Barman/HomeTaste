using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDailyRevenue
{
    public record GetDailyRevenueQuery(int Days = 30) : IRequest<Result<List<DailyRevenuePoint>>>;
}
