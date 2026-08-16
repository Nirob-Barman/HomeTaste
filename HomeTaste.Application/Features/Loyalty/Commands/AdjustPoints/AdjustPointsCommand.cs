using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Loyalty.Commands.AdjustPoints
{
    public class AdjustPointsCommand : IRequest<Result<bool>>
    {
        public AdjustPointsRequest Request { get; set; }

        public AdjustPointsCommand(AdjustPointsRequest request)
        {
            Request = request;
        }
    }
}
