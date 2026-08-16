using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Payments;
using HomeTaste.Application.Features.Payments.Commands.CancelPendingPayment;
using HomeTaste.Application.Features.Payments.Commands.ConfirmDirectPayment;
using HomeTaste.Application.Features.Payments.Commands.ConfirmPayment;
using HomeTaste.Application.Features.Payments.Commands.InitiatePayment;
using HomeTaste.Application.Features.Payments.Commands.RefundPayment;
using HomeTaste.Application.Features.Payments.Queries.GetAllPayments;
using HomeTaste.Application.Features.Payments.Queries.GetPaymentByOrderId;
using HomeTaste.Application.Features.Payments.Queries.GetPaymentById;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Handles payment initiation, confirmation, refunds, and history.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;

        public PaymentController(IMediator mediator, IConfiguration config)
        {
            _mediator = mediator;
            _config = config;
        }

        /// <summary>Initiates a payment for an order. Returns a pending transaction.</summary>
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
        {
            var callbackBaseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _mediator.Send(new InitiatePaymentCommand(request.OrderId, request.Gateway, request.Notes, callbackBaseUrl));
            return Ok(result);
        }

        /// <summary>Confirms a pending payment transaction (e.g. after gateway callback).</summary>
        [HttpPatch("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPaymentRequest request)
        {
            var result = await _mediator.Send(new ConfirmPaymentCommand(id, request.TransactionRef, request.Notes));
            return Ok(result);
        }

        /// <summary>Confirms a direct payment (Stripe/manual) and creates the transaction as Success.</summary>
        [HttpPost("confirm-direct")]
        public async Task<IActionResult> ConfirmDirect([FromBody] ConfirmDirectPaymentRequest request)
        {
            var result = await _mediator.Send(new ConfirmDirectPaymentCommand(request.OrderId, request.Gateway, request.TransactionRef, request.Notes));
            return Ok(result);
        }

        /// <summary>Refunds a successful payment. Admin only.</summary>
        [HttpPatch("{id:guid}/refund")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Refund(Guid id, [FromBody] RefundPaymentRequest request)
        {
            var result = await _mediator.Send(new RefundPaymentCommand(id, request.Notes));
            return Ok(result);
        }

        /// <summary>Gets the payment transaction for a specific order.</summary>
        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await _mediator.Send(new GetPaymentByOrderIdQuery(orderId));
            return Ok(result);
        }

        /// <summary>Gets a payment transaction by its ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPaymentByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Gets all payment transactions with optional status filter. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PaymentStatus? status = null)
        {
            var result = await _mediator.Send(new GetAllPaymentsQuery(pageNumber, pageSize, status));
            return Ok(result);
        }

        // ─── Redirect callbacks (called by payment providers after redirect-based flow) ──

        /// <summary>
        /// Provider redirects here on successful payment (e.g. bKash Checkout).
        /// Verifies with the provider then redirects the browser to the frontend success page.
        /// </summary>
        [HttpGet("callback/success")]
        [AllowAnonymous]
        public async Task<IActionResult> CallbackSuccess([FromQuery] Guid txId, [FromQuery] string gateway)
        {
            var frontendBase = _config["FrontendBaseUrl"] ?? "http://localhost:5173";
            try
            {
                await _mediator.Send(new ConfirmPaymentCommand(txId, null, null));
            }
            catch
            {
                return Redirect($"{frontendBase}/payment/cancel?txId={txId}&reason=verification_failed");
            }

            return Redirect($"{frontendBase}/payment/success?txId={txId}");
        }

        /// <summary>
        /// Provider redirects here when the user cancels (e.g. bKash Checkout cancel).
        /// Marks the transaction as Failed then redirects to the frontend cancel page.
        /// </summary>
        [HttpGet("callback/cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> CallbackCancel([FromQuery] Guid txId, [FromQuery] Guid? orderId)
        {
            await _mediator.Send(new CancelPendingPaymentCommand(txId));
            var frontendBase = _config["FrontendBaseUrl"] ?? "http://localhost:5173";
            var url = orderId.HasValue
                ? $"{frontendBase}/payment/cancel?txId={txId}&orderId={orderId}"
                : $"{frontendBase}/payment/cancel?txId={txId}";
            return Redirect(url);
        }

        /// <summary>Cancels a pending transaction (user chose a different payment method).</summary>
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _mediator.Send(new CancelPendingPaymentCommand(id));
            return Ok(result);
        }
    }
}
