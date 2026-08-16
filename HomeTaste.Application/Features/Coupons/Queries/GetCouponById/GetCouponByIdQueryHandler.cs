using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Queries.GetCouponById
{
    public class GetCouponByIdQueryHandler : IRequestHandler<GetCouponByIdQuery, Result<CouponResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetCouponByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CouponResponse>> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _context.Coupons.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (coupon == null)
                throw new NotFoundException("Coupon not found.");

            return Result<CouponResponse>.Ok(CouponMapper.ToResponse(coupon), "Coupon retrieved successfully");
        }
    }
}
