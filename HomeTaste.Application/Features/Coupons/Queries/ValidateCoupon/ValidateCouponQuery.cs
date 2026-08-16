using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.ValidateCoupon
{
    public class ValidateCouponQuery : IRequest<Result<CouponValidationResponse>>
    {
        public ValidateCouponRequest Request { get; set; }

        public ValidateCouponQuery(ValidateCouponRequest request)
        {
            Request = request;
        }
    }
}
