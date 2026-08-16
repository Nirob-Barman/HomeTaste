using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Loyalty.Queries.PreviewRedemption
{
    public class PreviewRedemptionQueryHandler : IRequestHandler<PreviewRedemptionQuery, Result<PointsPreviewResponse>>
    {
        private const int PointsRedemptionRate = 100; // 100 points = $1

        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public PreviewRedemptionQueryHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<PointsPreviewResponse>> Handle(PreviewRedemptionQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("Invalid user.");

            if (request.PointsToRedeem <= 0)
                throw new BadRequestException("Points to redeem must be greater than zero.");

            var account = await _context.LoyaltyAccounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

            var available = account?.CurrentPoints ?? 0;

            if (request.PointsToRedeem > available)
                throw new BadRequestException($"Insufficient points. Available: {available}.");

            var discount = Math.Round((decimal)request.PointsToRedeem / PointsRedemptionRate, 2);

            return Result<PointsPreviewResponse>.Ok(new PointsPreviewResponse
            {
                PointsToRedeem = request.PointsToRedeem,
                DiscountAmount = discount,
                RemainingPoints = available - request.PointsToRedeem
            }, "Redemption preview calculated.");
        }
    }
}
