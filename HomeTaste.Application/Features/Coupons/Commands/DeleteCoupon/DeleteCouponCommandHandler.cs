using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.DeleteCoupon
{
    public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCouponCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeleteCouponCommand command, CancellationToken cancellationToken)
        {
            var coupon = await _context.Coupons.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (coupon == null)
                throw new NotFoundException("Coupon not found.");

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Coupon deleted successfully");
        }
    }
}
