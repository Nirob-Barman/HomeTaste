using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;

namespace HomeTaste.Application.Features.Delivery.Assignments.Commands.UpdateDeliveryStatus
{
    public class UpdateDeliveryStatusCommandHandler : IRequestHandler<UpdateDeliveryStatusCommand, Result<DeliveryAssignmentResponse>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDeliveryStatusCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DeliveryAssignmentResponse>> Handle(UpdateDeliveryStatusCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var assignment = await _context.DeliveryAssignments.FindAsync(new object?[] { command.AssignmentId }, cancellationToken);
            if (assignment == null)
                throw new NotFoundException("Assignment not found.");

            var validationError = ValidateDeliveryStatusTransition(assignment.Status, request.Status);
            if (validationError != null)
                throw new BadRequestException(validationError);

            var personnel = await _context.DeliveryPersonnel.FindAsync(new object?[] { assignment.DeliveryPersonnelId }, cancellationToken);

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                assignment.UpdateStatus(request.Status, request.Notes);

                if (request.Status == DeliveryStatus.Delivered)
                {
                    var order = await _context.Orders.FindAsync(new object?[] { assignment.OrderId }, cancellationToken);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Delivered;
                        order.DeliveredAt = DateTime.UtcNow;
                        order.UpdatedAt = DateTime.UtcNow;
                    }

                    if (personnel != null)
                    {
                        personnel.SetAvailability(true);
                        personnel.RecordCompletedDelivery();
                    }
                }

                if (request.Status == DeliveryStatus.Failed && personnel != null)
                {
                    personnel.SetAvailability(true);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ServerErrorException("Failed to update delivery status. Please try again.");
            }

            return Result<DeliveryAssignmentResponse>.Ok(DeliveryMapper.ToResponse(assignment, personnel?.FullName), "Delivery status updated successfully.");
        }

        private static string? ValidateDeliveryStatusTransition(DeliveryStatus current, DeliveryStatus next)
        {
            var allowed = new Dictionary<DeliveryStatus, DeliveryStatus[]>
            {
                [DeliveryStatus.Assigned]  = [DeliveryStatus.PickedUp, DeliveryStatus.Failed],
                [DeliveryStatus.PickedUp]  = [DeliveryStatus.Delivered, DeliveryStatus.Failed],
                [DeliveryStatus.Delivered] = [],
                [DeliveryStatus.Failed]    = [],
            };

            if (!allowed.TryGetValue(current, out var allowedNext) || !allowedNext.Contains(next))
                return $"Cannot transition delivery from '{current}' to '{next}'.";

            return null;
        }
    }
}
