using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.DeleteCoupon
{
    public record DeleteCouponCommand(Guid Id) : IRequest<Result<bool>>;
}
