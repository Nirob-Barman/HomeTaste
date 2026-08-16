using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Delivery;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.AssignDelivery
{
    public class AssignDeliveryCommandHandler : IRequestHandler<AssignDeliveryCommand, Result<DeliveryAssignmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public AssignDeliveryCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryAssignmentResponse>> Handle(AssignDeliveryCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var order = await _context.Orders.FindAsync(new object?[] { request.OrderId }, cancellationToken);
            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                throw new BadRequestException($"Cannot assign delivery for a {order.Status} order.");

            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { request.DeliveryPersonnelId }, cancellationToken);
            if (personnel == null)
                throw new NotFoundException("Delivery personnel not found.");

            if (!personnel.IsAvailable)
                throw new BadRequestException("This delivery personnel is not available.");

            var existingAssignment = await _context.DeliveryAssignments
                .FirstOrDefaultAsync(a => a.OrderId == request.OrderId && a.Status != DeliveryStatus.Failed, cancellationToken);
            if (existingAssignment != null)
                throw new ConflictException("This order already has an active delivery assignment.");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var assignment = DeliveryAssignment.Create(request.OrderId, request.DeliveryPersonnelId);

                personnel.SetAvailability(false);

                if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Preparing || order.Status == OrderStatus.ReadyForPickup)
                {
                    order.Status = OrderStatus.OutForDelivery;
                    order.UpdatedAt = DateTime.UtcNow;
                }

                _context.DeliveryAssignments.Add(assignment);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return Result<DeliveryAssignmentResponse>.Ok(DeliveryMapper.ToResponse(assignment, personnel.FullName), "Delivery assigned successfully.");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to assign delivery. Please try again.");
            }
        }
    }
}
