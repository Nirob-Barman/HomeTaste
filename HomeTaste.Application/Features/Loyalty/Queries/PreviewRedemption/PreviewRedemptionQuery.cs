using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.PreviewRedemption
{
    public record PreviewRedemptionQuery(int PointsToRedeem) : IRequest<Result<PointsPreviewResponse>>;
}
