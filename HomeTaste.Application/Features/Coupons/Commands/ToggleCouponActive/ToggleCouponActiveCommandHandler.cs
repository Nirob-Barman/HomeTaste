using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.ToggleCouponActive
{
    public class ToggleCouponActiveCommandHandler : IRequestHandler<ToggleCouponActiveCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public ToggleCouponActiveCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(ToggleCouponActiveCommand command, CancellationToken cancellationToken)
        {
            var coupon = await _context.Coupons.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (coupon == null)
                throw new NotFoundException("Coupon not found.");

            coupon.ToggleActive();
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(coupon.IsActive, $"Coupon is now {(coupon.IsActive ? "active" : "inactive")}");
        }
    }
}
