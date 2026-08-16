using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.DeliveryZones;
using HomeTaste.Application.Features.DeliveryZones.Commands.CreateDeliveryZone;
using HomeTaste.Application.Features.DeliveryZones.Commands.DeleteDeliveryZone;
using HomeTaste.Application.Features.DeliveryZones.Commands.UpdateDeliveryZone;
using HomeTaste.Application.Features.DeliveryZones.Queries.CheckServiceability;
using HomeTaste.Application.Features.DeliveryZones.Queries.GetAllDeliveryZones;
using HomeTaste.Application.Features.DeliveryZones.Queries.GetDeliveryZoneById;
using MediatR;
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
        private readonly IMediator _mediator;

        public DeliveryZoneController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Returns all delivery zones. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllDeliveryZonesQuery());
            return Ok(result);
        }

        /// <summary>Returns a single delivery zone by ID. Admin only.</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDeliveryZoneByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Checks whether an address falls within a serviceable delivery zone.</summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckServiceability([FromQuery] Guid addressId)
        {
            var result = await _mediator.Send(new CheckServiceabilityQuery(addressId));
            return Ok(result);
        }

        /// <summary>Creates a new delivery zone. Admin only.</summary>
        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryZoneRequest request)
        {
            var result = await _mediator.Send(new CreateDeliveryZoneCommand(request));
            return StatusCode(201, result);
        }

        /// <summary>Updates an existing delivery zone. Admin only.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeliveryZoneRequest request)
        {
            var result = await _mediator.Send(new UpdateDeliveryZoneCommand(id, request));
            return Ok(result);
        }

        /// <summary>Deletes a delivery zone. Admin only.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDeliveryZoneCommand(id));
            return Ok(result);
        }
    }
}
