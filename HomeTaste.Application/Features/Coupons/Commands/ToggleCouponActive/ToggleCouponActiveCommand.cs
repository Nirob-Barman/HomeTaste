using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Coupons.Commands.ToggleCouponActive
{
    public record ToggleCouponActiveCommand(Guid Id) : IRequest<Result<bool>>;
}
