using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Validators.Delivery;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Delivery;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Delivery;
using HomeTaste.Domain.Enums;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;

namespace HomeTaste.Application.Services.Delivery
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public DeliveryService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>> GetAllDeliveryPersonnelAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _unitOfWork.Repository<DeliveryPersonnel>().GetAllAsQueryable();
            var totalCount = await _unitOfWork.Repository<DeliveryPersonnel>().CountAsync(query);

            var paged = _unitOfWork.Repository<DeliveryPersonnel>().PaginateAsQueryable(query, pageNumber, pageSize);
            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().ToEnumerableAsync(paged, p => MapPersonnelToResponse(p));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>> { Data = personnel, MetaData = meta },
                "Delivery personnel retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<DeliveryPersonnelResponse>> GetDeliveryPersonnelByIdAsync(Guid id)
        {
            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(id);
            if (personnel == null)
                return Result<DeliveryPersonnelResponse>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            return Result<DeliveryPersonnelResponse>.Ok(MapPersonnelToResponse(personnel), "Delivery personnel retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<DeliveryPersonnelResponse>> CreateDeliveryPersonnelAsync(CreateDeliveryPersonnelRequest request)
        {
            var errors = CreateDeliveryPersonnelRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<DeliveryPersonnelResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                var alreadyLinked = await _unitOfWork.Repository<DeliveryPersonnel>()
                    .AnyAsync(p => p.UserId == request.UserId);
                if (alreadyLinked)
                    return Result<DeliveryPersonnelResponse>.Fail("This user is already linked to a delivery personnel profile.", "Conflict", ResultType.Conflict);
            }

            var personnel = new DeliveryPersonnel
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FullName = request.FullName,
                Phone = request.Phone,
                VehicleType = request.VehicleType,
                VehicleNumber = request.VehicleNumber,
                IsAvailable = true,
                Rating = 0,
                TotalDeliveries = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<DeliveryPersonnel>().AddAsync(personnel);
            await _unitOfWork.SaveChangesAsync();

            return Result<DeliveryPersonnelResponse>.Ok(MapPersonnelToResponse(personnel), "Delivery personnel created successfully.", ResultType.Created);
        }

        public async Task<Result<DeliveryPersonnelResponse>> UpdateDeliveryPersonnelAsync(Guid id, UpdateDeliveryPersonnelRequest request)
        {
            var errors = UpdateDeliveryPersonnelRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<DeliveryPersonnelResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(id);
            if (personnel == null)
                return Result<DeliveryPersonnelResponse>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            personnel.FullName = request.FullName;
            personnel.Phone = request.Phone;
            personnel.VehicleType = request.VehicleType;
            personnel.VehicleNumber = request.VehicleNumber;
            personnel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return Result<DeliveryPersonnelResponse>.Ok(MapPersonnelToResponse(personnel), "Delivery personnel updated successfully.", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteDeliveryPersonnelAsync(Guid id)
        {
            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(id);
            if (personnel == null)
                return Result<bool>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            _unitOfWork.Repository<DeliveryPersonnel>().Remove(personnel);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Delivery personnel deleted successfully.", ResultType.Success);
        }

        public async Task<Result<bool>> ToggleAvailabilityAsync(Guid id)
        {
            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(id);
            if (personnel == null)
                return Result<bool>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            personnel.IsAvailable = !personnel.IsAvailable;
            personnel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(personnel.IsAvailable, $"Marked as {(personnel.IsAvailable ? "available" : "unavailable")}.", ResultType.Success);
        }

        public async Task<Result<bool>> UpdateLocationAsync(Guid id, UpdateLocationRequest request)
        {
            var errors = UpdateLocationRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<bool>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(id);
            if (personnel == null)
                return Result<bool>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            personnel.CurrentLatitude = request.Latitude;
            personnel.CurrentLongitude = request.Longitude;
            personnel.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Location updated successfully.", ResultType.Success);
        }

        public async Task<Result<DeliveryAssignmentResponse>> AssignDeliveryAsync(AssignDeliveryRequest request)
        {
            var errors = AssignDeliveryRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<DeliveryAssignmentResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(request.OrderId);
            if (order == null)
                return Result<DeliveryAssignmentResponse>.Fail("Order not found.", "Not found", ResultType.NotFound);

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return Result<DeliveryAssignmentResponse>.Fail($"Cannot assign delivery for a {order.Status} order.", "Bad request", ResultType.BadRequest);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(request.DeliveryPersonnelId);
            if (personnel == null)
                return Result<DeliveryAssignmentResponse>.Fail("Delivery personnel not found.", "Not found", ResultType.NotFound);

            if (!personnel.IsAvailable)
                return Result<DeliveryAssignmentResponse>.Fail("This delivery personnel is not available.", "Bad request", ResultType.BadRequest);

            var existingAssignment = await _unitOfWork.Repository<DeliveryAssignment>()
                .FirstOrDefaultAsync(a => a.OrderId == request.OrderId && a.Status != DeliveryStatus.Failed);
            if (existingAssignment != null)
                return Result<DeliveryAssignmentResponse>.Fail("This order already has an active delivery assignment.", "Conflict", ResultType.Conflict);

            await _unitOfWork.BeginTransaction();
            try
            {
                var assignment = new DeliveryAssignment
                {
                    Id = Guid.NewGuid(),
                    OrderId = request.OrderId,
                    DeliveryPersonnelId = request.DeliveryPersonnelId,
                    Status = DeliveryStatus.Assigned,
                    AssignedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                personnel.IsAvailable = false;
                personnel.UpdatedAt = DateTime.UtcNow;

                if (order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Preparing || order.Status == OrderStatus.ReadyForPickup)
                {
                    order.Status = OrderStatus.OutForDelivery;
                    order.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<OrderEntity>().Update(order);
                }

                await _unitOfWork.Repository<DeliveryAssignment>().AddAsync(assignment);
                _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return Result<DeliveryAssignmentResponse>.Ok(MapAssignmentToResponse(assignment, personnel.FullName), "Delivery assigned successfully.", ResultType.Created);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<DeliveryAssignmentResponse>.Fail("Failed to assign delivery. Please try again.", "Error", ResultType.Failure);
            }
        }

        public async Task<Result<DeliveryAssignmentResponse>> UpdateDeliveryStatusAsync(Guid assignmentId, UpdateDeliveryStatusRequest request)
        {
            var errors = UpdateDeliveryStatusRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<DeliveryAssignmentResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var assignment = await _unitOfWork.Repository<DeliveryAssignment>().GetByIdAsync(assignmentId);
            if (assignment == null)
                return Result<DeliveryAssignmentResponse>.Fail("Assignment not found.", "Not found", ResultType.NotFound);

            var validationError = ValidateDeliveryStatusTransition(assignment.Status, request.Status);
            if (validationError != null)
                return Result<DeliveryAssignmentResponse>.Fail(validationError, "Bad request", ResultType.BadRequest);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(assignment.DeliveryPersonnelId);

            await _unitOfWork.BeginTransaction();
            try
            {
                assignment.Status = request.Status;
                assignment.Notes = request.Notes ?? assignment.Notes;
                assignment.UpdatedAt = DateTime.UtcNow;

                if (request.Status == DeliveryStatus.PickedUp)
                    assignment.PickedUpAt = DateTime.UtcNow;

                if (request.Status == DeliveryStatus.Delivered)
                {
                    assignment.DeliveredAt = DateTime.UtcNow;

                    var order = await _unitOfWork.Repository<OrderEntity>().GetByIdAsync(assignment.OrderId);
                    if (order != null)
                    {
                        order.Status = OrderStatus.Delivered;
                        order.DeliveredAt = DateTime.UtcNow;
                        order.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<OrderEntity>().Update(order);
                    }

                    if (personnel != null)
                    {
                        personnel.IsAvailable = true;
                        personnel.TotalDeliveries++;
                        personnel.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);
                    }
                }

                if (request.Status == DeliveryStatus.Failed && personnel != null)
                {
                    personnel.IsAvailable = true;
                    personnel.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Repository<DeliveryPersonnel>().Update(personnel);
                }

                _unitOfWork.Repository<DeliveryAssignment>().Update(assignment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                return Result<DeliveryAssignmentResponse>.Fail("Failed to update delivery status. Please try again.", "Error", ResultType.Failure);
            }

            return Result<DeliveryAssignmentResponse>.Ok(MapAssignmentToResponse(assignment, personnel?.FullName), "Delivery status updated successfully.", ResultType.Success);
        }

        public async Task<Result<DeliveryAssignmentResponse>> GetDeliveryByOrderIdAsync(Guid orderId)
        {
            var assignment = await _unitOfWork.Repository<DeliveryAssignment>()
                .FirstOrDefaultAsync(a => a.OrderId == orderId);

            if (assignment == null)
                return Result<DeliveryAssignmentResponse>.Fail("No delivery assignment found for this order.", "Not found", ResultType.NotFound);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>().GetByIdAsync(assignment.DeliveryPersonnelId);
            return Result<DeliveryAssignmentResponse>.Ok(MapAssignmentToResponse(assignment, personnel?.FullName), "Delivery assignment retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<IEnumerable<DeliveryAssignmentResponse>>> GetMyAssignedDeliveriesAsync()
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<IEnumerable<DeliveryAssignmentResponse>>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var personnel = await _unitOfWork.Repository<DeliveryPersonnel>()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (personnel == null)
                return Result<IEnumerable<DeliveryAssignmentResponse>>.Fail("No delivery personnel profile found for this user.", "Not found", ResultType.NotFound);

            var assignments = await _unitOfWork.Repository<DeliveryAssignment>()
                .Where(a => a.DeliveryPersonnelId == personnel.Id);

            var response = assignments.Select(a => MapAssignmentToResponse(a, personnel.FullName));
            return Result<IEnumerable<DeliveryAssignmentResponse>>.Ok(response, "Assignments retrieved successfully.", ResultType.Success);
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

        private static DeliveryPersonnelResponse MapPersonnelToResponse(DeliveryPersonnel p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            FullName = p.FullName,
            Phone = p.Phone,
            VehicleType = p.VehicleType,
            VehicleNumber = p.VehicleNumber,
            IsAvailable = p.IsAvailable,
            CurrentLatitude = p.CurrentLatitude,
            CurrentLongitude = p.CurrentLongitude,
            Rating = p.Rating,
            TotalDeliveries = p.TotalDeliveries,
            CreatedAt = p.CreatedAt
        };

        private static DeliveryAssignmentResponse MapAssignmentToResponse(DeliveryAssignment a, string? personnelName) => new()
        {
            Id = a.Id,
            OrderId = a.OrderId,
            DeliveryPersonnelId = a.DeliveryPersonnelId,
            DeliveryPersonnelName = personnelName,
            Status = a.Status,
            StatusLabel = a.Status.ToString(),
            AssignedAt = a.AssignedAt,
            PickedUpAt = a.PickedUpAt,
            DeliveredAt = a.DeliveredAt,
            Notes = a.Notes
        };
    }
}
