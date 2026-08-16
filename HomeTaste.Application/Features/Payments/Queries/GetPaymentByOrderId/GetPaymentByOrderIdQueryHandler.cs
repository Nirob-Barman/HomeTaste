using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Payments.Queries.GetPaymentByOrderId
{
    public class GetPaymentByOrderIdQueryHandler : IRequestHandler<GetPaymentByOrderIdQuery, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetPaymentByOrderIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.OrderId == request.OrderId, cancellationToken);

            if (transaction == null)
                throw new NotFoundException("No payment found for this order.");

            return Result<PaymentTransactionResponse>.Ok(PaymentMapper.ToResponse(transaction), "Payment retrieved successfully.");
        }
    }
}
