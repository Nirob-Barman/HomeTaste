using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.GetCouponById
{
    public record GetCouponByIdQuery(Guid Id) : IRequest<Result<CouponResponse>>;
}
