using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetActivePaymentGateways
{
    public class GetActivePaymentGatewaysQueryHandler : IRequestHandler<GetActivePaymentGatewaysQuery, Result<List<PaymentGatewayResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfigEncryptor _encryptor;

        public GetActivePaymentGatewaysQueryHandler(IApplicationDbContext context, IConfigEncryptor encryptor)
        {
            _context = context;
            _encryptor = encryptor;
        }

        public async Task<Result<List<PaymentGatewayResponse>>> Handle(GetActivePaymentGatewaysQuery request, CancellationToken cancellationToken)
        {
            var activeGateways = await _context.PaymentGateways.Where(g => g.IsActive).ToListAsync(cancellationToken);
            var result = activeGateways.Select(g => PaymentGatewayConfigHelper.ToResponse(_encryptor, g)).ToList();
            return Result<List<PaymentGatewayResponse>>.Ok(result, "Active gateways retrieved.");
        }
    }
}
