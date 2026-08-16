using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.DeleteCoupon
{
    public class DeleteCouponCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public DeleteCouponCommand(Guid id)
        {
            Id = id;
        }
    }
}
