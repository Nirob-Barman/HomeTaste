using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.DeletePaymentGateway
{
    public class DeletePaymentGatewayCommandHandler : IRequestHandler<DeletePaymentGatewayCommand, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeletePaymentGatewayCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(DeletePaymentGatewayCommand command, CancellationToken cancellationToken)
        {
            var entity = await _context.PaymentGateways.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Gateway not found.");

            _context.PaymentGateways.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, "Gateway deleted successfully.");
        }
    }
}
