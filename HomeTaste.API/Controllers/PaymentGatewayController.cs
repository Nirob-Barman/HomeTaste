using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.PaymentGateways;
using HomeTaste.Application.Features.PaymentGateways.Commands.CreatePaymentGateway;
using HomeTaste.Application.Features.PaymentGateways.Commands.DeletePaymentGateway;
using HomeTaste.Application.Features.PaymentGateways.Commands.TogglePaymentGatewayActive;
using HomeTaste.Application.Features.PaymentGateways.Commands.UpdatePaymentGateway;
using HomeTaste.Application.Features.PaymentGateways.Queries.GetActivePaymentGateways;
using HomeTaste.Application.Features.PaymentGateways.Queries.GetAllPaymentGateways;
using HomeTaste.Application.Features.PaymentGateways.Queries.GetPaymentGatewayById;
using HomeTaste.Application.Features.PaymentGateways.Queries.GetSchema;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentGatewayController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("schema")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchema()
        {
            var result = await _mediator.Send(new GetSchemaQuery());
            return Ok(result);
        }

        // Customer-accessible: returns only active gateways (no secret keys exposed)
        [HttpGet("active")]
        [Authorize(Policy = Policies.AdminOrCustomer)]
        public async Task<IActionResult> GetActive()
        {
            var result = await _mediator.Send(new GetActivePaymentGatewaysQuery());
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllPaymentGatewaysQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPaymentGatewayByIdQuery(id));
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentGatewayRequest request)
        {
            var result = await _mediator.Send(new CreatePaymentGatewayCommand(request));
            return StatusCode(201, result);
        }

        [HttpPatch("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePaymentGatewayRequest request)
        {
            var result = await _mediator.Send(new UpdatePaymentGatewayCommand(id, request));
            return Ok(result);
        }

        [HttpPatch("{id:guid}/toggle")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var result = await _mediator.Send(new TogglePaymentGatewayActiveCommand(id));
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePaymentGatewayCommand(id));
            return Ok(result);
        }
    }
}
