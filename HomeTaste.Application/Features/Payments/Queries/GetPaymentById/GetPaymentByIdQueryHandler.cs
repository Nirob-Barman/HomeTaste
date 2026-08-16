using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetPaymentByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            return Result<PaymentTransactionResponse>.Ok(PaymentMapper.ToResponse(transaction), "Transaction retrieved successfully.");
        }
    }
}
