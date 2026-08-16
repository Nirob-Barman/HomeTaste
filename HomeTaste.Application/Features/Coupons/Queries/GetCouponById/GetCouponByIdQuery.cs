using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.GetCouponById
{
    public class GetCouponByIdQuery : IRequest<Result<CouponResponse>>
    {
        public Guid Id { get; set; }

        public GetCouponByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
