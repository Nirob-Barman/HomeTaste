using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Orders.Queries.GetDeliveryFee
{
    public class GetDeliveryFeeQueryHandler : IRequestHandler<GetDeliveryFeeQuery, Result<DeliveryFeeResponse>>
    {
        private readonly IDeliveryFeeService _deliveryFeeService;

        public GetDeliveryFeeQueryHandler(IDeliveryFeeService deliveryFeeService)
        {
            _deliveryFeeService = deliveryFeeService;
        }

        public Task<Result<DeliveryFeeResponse>> Handle(GetDeliveryFeeQuery request, CancellationToken cancellationToken)
        {
            var fee = _deliveryFeeService.Calculate(request.SubTotal);
            var result = Result<DeliveryFeeResponse>.Ok(
                new DeliveryFeeResponse { Fee = fee },
                "Delivery fee calculated.");
            return Task.FromResult(result);
        }
    }
}
