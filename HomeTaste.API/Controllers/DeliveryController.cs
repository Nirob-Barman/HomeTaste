using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Interfaces.Delivery;
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
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        // ── Personnel management (Admin) ──────────────────────────────

        /// <summary>Gets all delivery personnel with pagination. Admin only.</summary>
        [HttpGet("personnel")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAllPersonnel([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _deliveryService.GetAllDeliveryPersonnelAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a delivery personnel profile by ID. Admin only.</summary>
        [HttpGet("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetPersonnelById(Guid id)
        {
            var result = await _deliveryService.GetDeliveryPersonnelByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Creates a new delivery personnel profile. Admin only.</summary>
        [HttpPost("personnel")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> CreatePersonnel([FromBody] CreateDeliveryPersonnelRequest request)
        {
            var result = await _deliveryService.CreateDeliveryPersonnelAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates a delivery personnel profile. Admin only.</summary>
        [HttpPut("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> UpdatePersonnel(Guid id, [FromBody] UpdateDeliveryPersonnelRequest request)
        {
            var result = await _deliveryService.UpdateDeliveryPersonnelAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes a delivery personnel profile. Admin only.</summary>
        [HttpDelete("personnel/{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> DeletePersonnel(Guid id)
        {
            var result = await _deliveryService.DeleteDeliveryPersonnelAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Toggles a delivery personnel's availability. Admin only.</summary>
        [HttpPatch("personnel/{id:guid}/toggle-availability")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> ToggleAvailability(Guid id)
        {
            var result = await _deliveryService.ToggleAvailabilityAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        // ── Location update (DeliveryPersonnel) ──────────────────────

        /// <summary>Updates the real-time GPS location of a delivery personnel.</summary>
        [HttpPatch("personnel/{id:guid}/location")]
        [Authorize(Policy = Policies.DeliveryPersonnelOnly)]
        public async Task<IActionResult> UpdateLocation(Guid id, [FromBody] UpdateLocationRequest request)
        {
            var result = await _deliveryService.UpdateLocationAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        // ── Assignment management (Admin) ─────────────────────────────

        /// <summary>Assigns a delivery personnel to an order. Admin only.</summary>
        [HttpPost("assign")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Assign([FromBody] AssignDeliveryRequest request)
        {
            var result = await _deliveryService.AssignDeliveryAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Advances a delivery assignment through its status workflow (PickedUp → Delivered).</summary>
        [HttpPatch("assignments/{assignmentId:guid}/status")]
        [Authorize(Policy = Policies.AdminOrDelivery)]
        public async Task<IActionResult> UpdateDeliveryStatus(Guid assignmentId, [FromBody] UpdateDeliveryStatusRequest request)
        {
            var result = await _deliveryService.UpdateDeliveryStatusAsync(assignmentId, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets the delivery assignment for a specific order.</summary>
        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await _deliveryService.GetDeliveryByOrderIdAsync(orderId);
            return ApiResponseMapper.FromResult(this, result);
        }

        // ── Delivery personnel self-service ───────────────────────────

        /// <summary>Gets all assignments for the currently authenticated delivery personnel.</summary>
        [HttpGet("my-deliveries")]
        [Authorize(Policy = Policies.DeliveryPersonnelOnly)]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var result = await _deliveryService.GetMyAssignedDeliveriesAsync();
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
