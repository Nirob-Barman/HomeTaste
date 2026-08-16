using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetSchema
{
    public class GetSchemaQuery : IRequest<Result<List<GatewayFamilyResponse>>>
    {
    }
}
