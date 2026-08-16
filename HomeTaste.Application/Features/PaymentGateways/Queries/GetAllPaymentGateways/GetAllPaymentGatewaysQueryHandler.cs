using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetAllPaymentGateways
{
    public class GetAllPaymentGatewaysQueryHandler : IRequestHandler<GetAllPaymentGatewaysQuery, Result<List<PaymentGatewayResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfigEncryptor _encryptor;

        public GetAllPaymentGatewaysQueryHandler(IApplicationDbContext context, IConfigEncryptor encryptor)
        {
            _context = context;
            _encryptor = encryptor;
        }

        public async Task<Result<List<PaymentGatewayResponse>>> Handle(GetAllPaymentGatewaysQuery request, CancellationToken cancellationToken)
        {
            var all = await _context.PaymentGateways.ToListAsync(cancellationToken);
            var result = all.Select(g => PaymentGatewayConfigHelper.ToResponse(_encryptor, g)).ToList();
            return Result<List<PaymentGatewayResponse>>.Ok(result, "Gateways retrieved.");
        }
    }
}
