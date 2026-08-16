using HomeTaste.Application.Features.Addresses;
using HomeTaste.Application.Features.Addresses.Commands.CreateAddress;
using HomeTaste.Application.Features.Addresses.Commands.DeleteAddress;
using HomeTaste.Application.Features.Addresses.Commands.SetDefaultAddress;
using HomeTaste.Application.Features.Addresses.Commands.UpdateAddress;
using HomeTaste.Application.Features.Addresses.Queries.GetAddressById;
using HomeTaste.Application.Features.Addresses.Queries.GetMyAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Manages delivery addresses for the authenticated customer.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Gets all addresses belonging to the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var result = await _mediator.Send(new GetMyAddressesQuery());
            return Ok(result);
        }

        /// <summary>Gets a single address by ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetAddressByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Creates a new address for the current user.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressRequest request)
        {
            var result = await _mediator.Send(new CreateAddressCommand(request));
            return StatusCode(201, result);
        }

        /// <summary>Updates an existing address.</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AddressRequest request)
        {
            var result = await _mediator.Send(new UpdateAddressCommand(id, request));
            return Ok(result);
        }

        /// <summary>Deletes an address.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteAddressCommand(id));
            return Ok(result);
        }

        /// <summary>Sets an address as the default delivery address.</summary>
        [HttpPatch("{id:guid}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _mediator.Send(new SetDefaultAddressCommand(id));
            return Ok(result);
        }
    }
}
