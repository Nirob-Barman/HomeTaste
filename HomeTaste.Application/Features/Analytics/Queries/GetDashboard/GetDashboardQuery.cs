using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDashboard
{
    public record GetDashboardQuery : IRequest<Result<DashboardStatsResponse>>;
}
