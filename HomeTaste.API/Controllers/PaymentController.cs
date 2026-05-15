using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Interfaces.Payment;
using HomeTaste.Domain.Enums;
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
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        /// <summary>Initiates a payment for an order. Returns a pending transaction.</summary>
        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
        {
            var callbackBaseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await _paymentService.InitiatePaymentAsync(request, callbackBaseUrl);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Confirms a pending payment transaction (e.g. after gateway callback).</summary>
        [HttpPatch("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPaymentRequest request)
        {
            var result = await _paymentService.ConfirmPaymentAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Refunds a successful payment. Admin only.</summary>
        [HttpPatch("{id:guid}/refund")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Refund(Guid id, [FromBody] RefundPaymentRequest request)
        {
            var result = await _paymentService.RefundPaymentAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets the payment transaction for a specific order.</summary>
        [HttpGet("order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var result = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a payment transaction by its ID.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets all payment transactions with optional status filter. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PaymentStatus? status = null)
        {
            var result = await _paymentService.GetAllPaymentsAsync(pageNumber, pageSize, status);
            return ApiResponseMapper.FromResult(this, result);
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
            var confirmResult = await _paymentService.ConfirmPaymentAsync(txId, new ConfirmPaymentRequest());
            if (!confirmResult.Success)
                return Redirect($"{frontendBase}/payment/cancel?txId={txId}&reason=verification_failed");

            return Redirect($"{frontendBase}/payment/success?txId={txId}");
        }

        /// <summary>
        /// Provider redirects here when the user cancels (e.g. bKash Checkout cancel).
        /// Redirects the browser to the frontend cancel page.
        /// </summary>
        [HttpGet("callback/cancel")]
        [AllowAnonymous]
        public IActionResult CallbackCancel([FromQuery] Guid txId)
        {
            var frontendBase = _config["FrontendBaseUrl"] ?? "http://localhost:5173";
            return Redirect($"{frontendBase}/payment/cancel?txId={txId}");
        }
    }
}
