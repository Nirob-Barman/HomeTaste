using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetSchema
{
    public record GetSchemaQuery : IRequest<Result<List<GatewayFamilyResponse>>>;
}
