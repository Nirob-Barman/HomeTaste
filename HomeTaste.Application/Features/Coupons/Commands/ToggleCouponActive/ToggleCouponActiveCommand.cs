using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.ToggleCouponActive
{
    public class ToggleCouponActiveCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public ToggleCouponActiveCommand(Guid id)
        {
            Id = id;
        }
    }
}
