using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DeliveryPersonnelEntity = HomeTaste.Domain.Entities.Delivery.DeliveryPersonnel;

namespace HomeTaste.Application.Features.Delivery.Personnel.Commands.CreateDeliveryPersonnel
{
    public class CreateDeliveryPersonnelCommandHandler : IRequestHandler<CreateDeliveryPersonnelCommand, Result<DeliveryPersonnelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CreateDeliveryPersonnelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryPersonnelResponse>> Handle(CreateDeliveryPersonnelCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                var alreadyLinked = await _context.DeliveryPersonnel.AnyAsync(p => p.UserId == request.UserId, cancellationToken);
                if (alreadyLinked)
                    throw new ConflictException("This user is already linked to a delivery personnel profile.");
            }

            var personnel = DeliveryPersonnelEntity.Create(request.UserId, request.FullName, request.Phone, request.VehicleType, request.VehicleNumber);

            _context.DeliveryPersonnel.Add(personnel);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<DeliveryPersonnelResponse>.Ok(DeliveryMapper.ToResponse(personnel), "Delivery personnel created successfully.");
        }
    }
}
