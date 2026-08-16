using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDashboard
{
    public class GetDashboardQuery : IRequest<Result<DashboardStatsResponse>>
    {
    }
}
