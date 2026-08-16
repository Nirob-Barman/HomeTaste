using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.UpdatePaymentGateway
{
    public class UpdatePaymentGatewayCommandHandler : IRequestHandler<UpdatePaymentGatewayCommand, Result<PaymentGatewayResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IConfigEncryptor _encryptor;

        public UpdatePaymentGatewayCommandHandler(IApplicationDbContext context, IUserContextService userContextService, IConfigEncryptor encryptor)
        {
            _context = context;
            _userContextService = userContextService;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentGatewayResponse>> Handle(UpdatePaymentGatewayCommand command, CancellationToken cancellationToken)
        {
            var entity = await _context.PaymentGateways.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Gateway not found.");

            Guid.TryParse(_userContextService.UserId, out var userId);

            var mergedConfig = PaymentGatewayConfigHelper.MergeConfig(_encryptor, entity.Config, command.Config);

            entity.UpdateDetails(
                command.Name.Trim(),
                command.IsActive,
                command.IsSandbox,
                mergedConfig,
                userId == Guid.Empty ? null : userId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<PaymentGatewayResponse>.Ok(PaymentGatewayConfigHelper.ToResponse(_encryptor, entity), "Gateway updated successfully.");
        }
    }
}
