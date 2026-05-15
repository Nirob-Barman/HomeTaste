using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Order;
using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    /// <summary>
    /// Handles order placement, status tracking, and admin order management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryFeeService _deliveryFeeService;
        private readonly IPdfInvoiceService _pdfInvoiceService;

        public OrderController(IOrderService orderService, IDeliveryFeeService deliveryFeeService, IPdfInvoiceService pdfInvoiceService)
        {
            _orderService = orderService;
            _deliveryFeeService = deliveryFeeService;
            _pdfInvoiceService = pdfInvoiceService;
        }

        /// <summary>Returns the delivery fee for a given subtotal before placing an order.</summary>
        [HttpGet("delivery-fee")]
        public IActionResult GetDeliveryFee([FromQuery] decimal subTotal)
        {
            var fee = _deliveryFeeService.Calculate(subTotal);
            var result = Result<DeliveryFeeResponse>.Ok(
                new DeliveryFeeResponse { Fee = fee },
                "Delivery fee calculated.",
                ResultType.Success);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets the authenticated customer's orders with pagination.</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetMyOrdersAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets a single order by ID. Customers see only their own orders.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Gets all orders with optional status filter and pagination. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null)
        {
            var result = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, status);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Places a new order for the authenticated customer.</summary>
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            var result = await _orderService.PlaceOrderAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Advances an order through its status workflow. Admin only.</summary>
        [HttpPatch("{id:guid}/status")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Cancels an order. Customers can cancel Pending/Confirmed orders; Admins can cancel any.</summary>
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request)
        {
            var result = await _orderService.CancelOrderAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Downloads a PDF invoice for the given order.</summary>
        [HttpGet("{id:guid}/invoice")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.Success || result.Data == null)
                return ApiResponseMapper.FromResult(this, result);

            var pdf = _pdfInvoiceService.Generate(result.Data);
            var fileName = $"invoice-{id.ToString()[..8].ToUpper()}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
