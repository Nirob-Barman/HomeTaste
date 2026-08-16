using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CouponEntity = HomeTaste.Domain.Entities.Coupon.Coupon;

namespace HomeTaste.Application.Features.Coupons.Commands.CreateCoupon
{
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, Result<CouponResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateCouponCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CouponResponse>> Handle(CreateCouponCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var code = request.Code?.Trim().ToUpperInvariant();

            var exists = await _context.Coupons.AnyAsync(c => c.Code == code, cancellationToken);
            if (exists)
                throw new ConflictException("A coupon with this code already exists.");

            var coupon = CouponEntity.Create(
                code,
                request.Description,
                request.DiscountType,
                request.DiscountValue,
                request.MinOrderAmount,
                request.MaxDiscountAmount,
                request.UsageLimit,
                request.ExpiresAt,
                request.IsActive,
                request.IsFirstOrderOnly);

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CouponResponse>.Ok(CouponMapper.ToResponse(coupon), "Coupon created successfully");
        }
    }
}
