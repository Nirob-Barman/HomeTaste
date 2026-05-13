using HomeTaste.API.Wrappers;
using HomeTaste.Application.DTOs.Address;
using HomeTaste.Application.Interfaces.Address;
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
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>Gets all addresses belonging to the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var result = await _addressService.GetMyAddressesAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a single address by ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _addressService.GetAddressByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Creates a new address for the current user.</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddressRequest request)
        {
            var result = await _addressService.CreateAddressAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Updates an existing address.</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AddressRequest request)
        {
            var result = await _addressService.UpdateAddressAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes an address.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Sets an address as the default delivery address.</summary>
        [HttpPatch("{id:guid}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _addressService.SetDefaultAddressAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
