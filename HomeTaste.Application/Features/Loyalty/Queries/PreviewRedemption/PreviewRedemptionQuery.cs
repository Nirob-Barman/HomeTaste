using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Queries.PreviewRedemption
{
    public class PreviewRedemptionQuery : IRequest<Result<PointsPreviewResponse>>
    {
        public int PointsToRedeem { get; set; }

        public PreviewRedemptionQuery(int pointsToRedeem)
        {
            PointsToRedeem = pointsToRedeem;
        }
    }
}
