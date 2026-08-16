using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Commands.AdjustPoints
{
    public record AdjustPointsCommand(string? UserId, int Points, string? Description) : IRequest<Result<bool>>;
}
