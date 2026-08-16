using HomeTaste.Application.Payment;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetSchema
{
    public class GetSchemaQueryHandler : IRequestHandler<GetSchemaQuery, Result<List<GatewayFamilyResponse>>>
    {
        public Task<Result<List<GatewayFamilyResponse>>> Handle(GetSchemaQuery request, CancellationToken cancellationToken)
        {
            var schema = GatewayConfigSchema.Families.Select(f => new GatewayFamilyResponse(
                f.Key,
                f.DisplayName,
                f.Variants.Select(v => new GatewayVariantResponse(
                    v.Slug,
                    v.DisplayName,
                    v.VariantLabel,
                    v.Fields.Select(fd => new GatewayFieldResponse(
                        fd.Key,
                        fd.Label,
                        fd.IsSecret,
                        fd.IsRequired,
                        fd.Placeholder)).ToList())).ToList())).ToList();

            return Task.FromResult(Result<List<GatewayFamilyResponse>>.Ok(schema, "Schema retrieved."));
        }
    }
}
