using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.UpdateCoupon
{
    public class UpdateCouponCommand : IRequest<Result<CouponResponse>>
    {
        public Guid Id { get; set; }
        public CouponRequest Request { get; set; }

        public UpdateCouponCommand(Guid id, CouponRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
