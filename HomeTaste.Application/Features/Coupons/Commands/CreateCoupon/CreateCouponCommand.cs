using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.CreateCoupon
{
    public class CreateCouponCommand : IRequest<Result<CouponResponse>>
    {
        public CouponRequest Request { get; set; }

        public CreateCouponCommand(CouponRequest request)
        {
            Request = request;
        }
    }
}
