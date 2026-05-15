using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Interfaces.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Manages delivery zones and checks address serviceability.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeliveryZoneController : ControllerBase
    {
        private readonly IDeliveryZoneService _service;

        public DeliveryZoneController(IDeliveryZoneService service)
        {
            _service = service;
        }

        /// <summary>Returns all delivery zones. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns a single delivery zone by ID. Admin only.</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Checks whether an address falls within a serviceable delivery zone.</summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckServiceability([FromQuery] Guid addressId)
        {
            var result = await _service.CheckServiceabilityAsync(addressId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Creates a new delivery zone. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryZoneRequest request)
        {
            var result = await _service.CreateAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates an existing delivery zone. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryZoneRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes a delivery zone. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
