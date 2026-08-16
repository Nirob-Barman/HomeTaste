using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Queries.GetPaymentGatewayById
{
    public class GetPaymentGatewayByIdQueryHandler : IRequestHandler<GetPaymentGatewayByIdQuery, Result<PaymentGatewayResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfigEncryptor _encryptor;

        public GetPaymentGatewayByIdQueryHandler(IApplicationDbContext context, IConfigEncryptor encryptor)
        {
            _context = context;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentGatewayResponse>> Handle(GetPaymentGatewayByIdQuery request, CancellationToken cancellationToken)
        {
            var gateway = await _context.PaymentGateways.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (gateway == null)
                throw new NotFoundException("Gateway not found.");

            return Result<PaymentGatewayResponse>.Ok(PaymentGatewayConfigHelper.ToResponse(_encryptor, gateway), "Gateway retrieved.");
        }
    }
}
