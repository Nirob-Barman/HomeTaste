using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IPaymentGatewayService _gatewayService;

        public PaymentGatewayController(IPaymentGatewayService gatewayService)
        {
            _gatewayService = gatewayService;
        }

        // Customer-accessible: returns only active gateways (no secret keys exposed)
        [HttpGet("active")]
        [Authorize(Policy = Policies.AdminOrCustomer)]
        public async Task<IActionResult> GetActive()
        {
            var result = await _gatewayService.GetActiveAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _gatewayService.GetAllAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _gatewayService.GetByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentGatewayRequest request)
        {
            var result = await _gatewayService.CreateAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentGatewayRequest request)
        {
            var result = await _gatewayService.UpdateAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpPatch("{id:guid}/toggle")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _gatewayService.ToggleActiveAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _gatewayService.DeleteAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
