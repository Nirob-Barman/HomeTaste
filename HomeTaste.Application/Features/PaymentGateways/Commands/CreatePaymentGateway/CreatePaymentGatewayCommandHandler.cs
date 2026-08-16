using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Payment;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GatewayEntity = HomeTaste.Domain.Entities.Payment.PaymentGateway;

namespace HomeTaste.Application.Features.PaymentGateways.Commands.CreatePaymentGateway
{
    public class CreatePaymentGatewayCommandHandler : IRequestHandler<CreatePaymentGatewayCommand, Result<PaymentGatewayResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IConfigEncryptor _encryptor;

        public CreatePaymentGatewayCommandHandler(IApplicationDbContext context, IUserContextService userContextService, IConfigEncryptor encryptor)
        {
            _context = context;
            _userContextService = userContextService;
            _encryptor = encryptor;
        }

        public async Task<Result<PaymentGatewayResponse>> Handle(CreatePaymentGatewayCommand command, CancellationToken cancellationToken)
        {
            var slug = command.Slug.Trim().ToLowerInvariant();

            var variant = GatewayConfigSchema.FindVariant(slug);
            if (variant == null)
                throw new BadRequestException($"Unknown gateway slug '{slug}'.");

            var missingField = variant.Fields.FirstOrDefault(f => f.IsRequired && !command.Config.ContainsKey(f.Key));
            if (missingField != null)
                throw new BadRequestException($"'{missingField.Label}' is required.");

            var exists = await _context.PaymentGateways.AnyAsync(g => g.Slug == slug, cancellationToken);
            if (exists)
                throw new ConflictException($"A gateway with slug '{slug}' already exists.");

            var family = GatewayConfigSchema.FindFamily(slug)!;
            Guid.TryParse(_userContextService.UserId, out var userId);

            var configJson = PaymentGatewayConfigHelper.BuildConfigJson(command.Config);
            var entity = GatewayEntity.Create(
                command.Name.Trim(),
                family.Key,
                slug,
                _encryptor.Encrypt(configJson),
                command.IsActive,
                command.IsSandbox,
                userId == Guid.Empty ? null : userId);

            _context.PaymentGateways.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<PaymentGatewayResponse>.Ok(PaymentGatewayConfigHelper.ToResponse(_encryptor, entity), "Gateway created successfully.");
        }
    }
}
