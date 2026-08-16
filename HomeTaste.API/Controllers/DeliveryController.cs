using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Delivery;
using HomeTaste.Application.Features.Delivery.Assignments.Commands.AssignDelivery;
using HomeTaste.Application.Features.Delivery.Assignments.Commands.UpdateDeliveryStatus;
using HomeTaste.Application.Features.Delivery.Assignments.Queries.GetDeliveryByOrderId;
using HomeTaste.Application.Features.Delivery.Assignments.Queries.GetMyAssignedDeliveries;
using HomeTaste.Application.Features.Delivery.Personnel.Commands.CreateDeliveryPersonnel;
using HomeTaste.Application.Features.Delivery.Personnel.Commands.DeleteDeliveryPersonnel;
using HomeTaste.Application.Features.Delivery.Personnel.Commands.ToggleAvailability;
using HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateDeliveryPersonnel;
using HomeTaste.Application.Features.Delivery.Personnel.Commands.UpdateLocation;
using HomeTaste.Application.Features.Delivery.Personnel.Queries.GetAllDeliveryPersonnel;
using HomeTaste.Application.Features.Delivery.Personnel.Queries.GetDeliveryPersonnelById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Manages delivery personnel, assignments, real-time location updates, and status tracking.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ── Personnel management (Admin) ──────────────────────────────

        /// <summary>Gets all delivery personnel with pagination. Admin only.</summary>
        [HttpGet("personnel")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAllPersonnel([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllDeliveryPersonnelQuery { PageNumber = pageNumber, PageSize = pageSize });
            return Ok(result);
        }

        /// <summary>Gets a delivery personnel profile by ID. Admin only.</summary>
        [HttpGet("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetPersonnelById(Guid id)
        {
            var result = await _mediator.Send(new GetDeliveryPersonnelByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Creates a new delivery personnel profile. Admin only.</summary>
        [HttpPost("personnel")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> CreatePersonnel([FromBody] CreateDeliveryPersonnelRequest request)
        {
            var result = await _mediator.Send(new CreateDeliveryPersonnelCommand(request.UserId, request.FullName, request.Phone, request.VehicleType, request.VehicleNumber));
            return StatusCode(201, result);
        }

        /// <summary>Updates a delivery personnel profile. Admin only.</summary>
        [HttpPut("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> UpdatePersonnel(Guid id, [FromBody] UpdateDeliveryPersonnelRequest request)
        {
            var result = await _mediator.Send(new UpdateDeliveryPersonnelCommand(id, request.FullName, request.Phone, request.VehicleType, request.VehicleNumber));
            return Ok(result);
        }

        /// <summary>Deletes a delivery personnel profile. Admin only.</summary>
        [HttpDelete("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> DeletePersonnel(Guid id)
        {
            var result = await _mediator.Send(new DeleteDeliveryPersonnelCommand(id));
            return Ok(result);
        }

        /// <summary>Toggles a delivery personnel's availability. Admin only.</summary>
        [HttpPatch("personnel/{id:guid}/toggle-availability")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> ToggleAvailability(Guid id)
        {
            var result = await _mediator.Send(new ToggleAvailabilityCommand(id));
            return Ok(result);
        }

        // ── Location update (DeliveryPersonnel) ──────────────────────

        /// <summary>Updates the real-time GPS location of a delivery personnel.</summary>
        [HttpPatch("personnel/{id:guid}/location")]
        [Authorize(Policy = Policies.DeliveryPersonnelOnly)]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationRequest request)
        {
            var result = await _mediator.Send(new UpdateLocationCommand(id, request.Latitude, request.Longitude));
            return Ok(result);
        }

        // ── Assignment management (Admin) ─────────────────────────────

        /// <summary>Assigns a delivery personnel to an order. Admin only.</summary>
        [HttpPost("assign")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Assign([FromBody] AssignDeliveryRequest request)
        {
            var result = await _mediator.Send(new AssignDeliveryCommand(request.OrderId, request.DeliveryPersonnelId));
            return StatusCode(201, result);
        }

        /// <summary>Advances a delivery assignment through its status workflow (PickedUp → Delivered).</summary>
        [HttpPatch("assignments/{assignmentId:guid}/status")]
        [Authorize(Policy = Policies.AdminOrDelivery)]
        public async Task<IActionResult> UpdateDeliveryStatus(Guid assignmentId, [FromBody] UpdateDeliveryStatusRequest request)
        {
            var result = await _mediator.Send(new UpdateDeliveryStatusCommand(assignmentId, request.Status, request.Notes));
            return Ok(result);
        }

        /// <summary>Gets the delivery assignment for a specific order.</summary>
        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await _mediator.Send(new GetDeliveryByOrderIdQuery(orderId));
            return Ok(result);
        }

        // ── Delivery personnel self-service ───────────────────────────

        /// <summary>Gets all assignments for the currently authenticated delivery personnel.</summary>
        [HttpGet("my-deliveries")]
        [Authorize(Policy = Policies.DeliveryPersonnelOnly)]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var result = await _mediator.Send(new GetMyAssignedDeliveriesQuery());
            return Ok(result);
        }
    }
}
