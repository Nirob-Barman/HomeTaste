using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Coupons.Commands.UpdateCoupon
{
    public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, Result<CouponResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCouponCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CouponResponse>> Handle(UpdateCouponCommand command, CancellationToken cancellationToken)
        {
            var coupon = await _context.Coupons.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (coupon == null)
                throw new NotFoundException("Coupon not found.");

            var code = command.Code?.Trim().ToUpperInvariant();
            var duplicate = await _context.Coupons.AnyAsync(c => c.Code == code && c.Id != command.Id, cancellationToken);
            if (duplicate)
                throw new ConflictException("A coupon with this code already exists.");

            coupon.UpdateDetails(
                code,
                command.Description,
                command.DiscountType,
                command.DiscountValue,
                command.MinOrderAmount,
                command.MaxDiscountAmount,
                command.UsageLimit,
                command.ExpiresAt,
                command.IsActive,
                command.IsFirstOrderOnly);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CouponResponse>.Ok(CouponMapper.ToResponse(coupon), "Coupon updated successfully");
        }
    }
}
