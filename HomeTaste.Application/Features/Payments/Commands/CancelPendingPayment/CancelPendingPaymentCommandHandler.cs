using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.CancelPendingPayment
{
    public class CancelPendingPaymentCommandHandler : IRequestHandler<CancelPendingPaymentCommand, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public CancelPendingPaymentCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(CancelPendingPaymentCommand command, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(new object?[] { command.TransactionId }, cancellationToken);
            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            if (transaction.Status != PaymentStatus.Pending)
                throw new BadRequestException("Only pending transactions can be cancelled.");

            // Skip ownership for anonymous callers (gateway cancel callbacks)
            Guid.TryParse(_userContextService.UserId, out var userId);
            if (userId != Guid.Empty)
            {
                var order = await _context.Orders.FindAsync(new object?[] { transaction.OrderId }, cancellationToken);
                if (order == null)
                    throw new NotFoundException("Associated order not found.");
                if (order.UserId != userId && !_userContextService.IsInRole("Admin"))
                    throw new ForbiddenAccessException("Access denied.");
            }

            transaction.MarkFailed();
            await _context.SaveChangesAsync(cancellationToken);

            return Result<PaymentTransactionResponse>.Ok(PaymentMapper.ToResponse(transaction), "Payment cancelled.");
        }
    }
}
