using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetPaymentGatewayById
{
    public class GetPaymentGatewayByIdQuery : IRequest<Result<PaymentGatewayResponse>>
    {
        public Guid Id { get; set; }

        public GetPaymentGatewayByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
