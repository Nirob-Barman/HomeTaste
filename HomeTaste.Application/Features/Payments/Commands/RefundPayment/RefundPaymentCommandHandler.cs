using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<PaymentTransactionResponse>>
    {
        private readonly IApplicationDbContext _context;

        public RefundPaymentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<PaymentTransactionResponse>> Handle(RefundPaymentCommand command, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(new object?[] { command.TransactionId }, cancellationToken);
            if (transaction == null)
                throw new NotFoundException("Transaction not found.");

            if (transaction.Status != PaymentStatus.Success)
                throw new BadRequestException("Only successful payments can be refunded.");

            var order = await _context.Orders.FindAsync(new object?[] { transaction.OrderId }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Associated order not found.");

            await using var transactionScope = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                transaction.MarkRefunded(command.Notes);
                order.UpdateStatus(OrderStatus.Refunded);

                await _context.SaveChangesAsync(cancellationToken);
                await transactionScope.CommitAsync(cancellationToken);
            }
            catch
            {
                await transactionScope.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to process refund. Please try again.");
            }

            return Result<PaymentTransactionResponse>.Ok(PaymentMapper.ToResponse(transaction), "Refund processed successfully.");
        }
    }
}
