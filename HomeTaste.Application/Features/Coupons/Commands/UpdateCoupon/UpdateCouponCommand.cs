using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.UpdateCoupon
{
    public record UpdateCouponCommand(
        Guid Id,
        string? Code,
        string? Description,
        DiscountType DiscountType,
        decimal DiscountValue,
        decimal? MinOrderAmount,
        decimal? MaxDiscountAmount,
        int? UsageLimit,
        DateTime? ExpiresAt,
        bool IsActive,
        bool IsFirstOrderOnly) : IRequest<Result<CouponResponse>>;
}
