using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Payment;
using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.TogglePaymentGatewayActive
{
    public class TogglePaymentGatewayActiveCommandHandler : IRequestHandler<TogglePaymentGatewayActiveCommand, Result<PaymentGatewayResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IConfigEncryptor _encryptor;

        public TogglePaymentGatewayActiveCommandHandler(IApplicationDbContext context, IUserContextService userContextService, IConfigEncryptor encryptor)
        {
            _context = context;
            _userContextService = userContextService;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentGatewayResponse>> Handle(TogglePaymentGatewayActiveCommand command, CancellationToken cancellationToken)
        {
            var entity = await _context.PaymentGateways.FindAsync(new object?[] { command.Id }, cancellationToken);
            if (entity == null)
                throw new NotFoundException("Gateway not found.");

            var config = PaymentGatewayConfigHelper.DecryptConfig(_encryptor, entity.Config);
            var variant = GatewayConfigSchema.FindVariant(entity.Slug);
            if (!entity.IsActive && variant != null)
            {
                var missingField = variant.Fields.FirstOrDefault(f => f.IsRequired && !config.ContainsKey(f.Key));
                if (missingField != null)
                    throw new BadRequestException($"Cannot activate: '{missingField.Label}' is not configured.");
            }

            Guid.TryParse(_userContextService.UserId, out var userId);
            entity.ToggleActive(userId == Guid.Empty ? null : userId);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<PaymentGatewayResponse>.Ok(
                PaymentGatewayConfigHelper.ToResponse(_encryptor, entity),
                $"Gateway is now {(entity.IsActive ? "active" : "inactive")}.");
        }
    }
}
