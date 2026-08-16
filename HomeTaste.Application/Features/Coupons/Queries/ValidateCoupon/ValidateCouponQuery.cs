using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.ValidateCoupon
{
    public record ValidateCouponQuery(string? Code, decimal OrderAmount) : IRequest<Result<CouponValidationResponse>>;
}
