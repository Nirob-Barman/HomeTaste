using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Orders;
using HomeTaste.Application.Features.Orders.Commands.CancelOrder;
using HomeTaste.Application.Features.Orders.Commands.PlaceOrder;
using HomeTaste.Application.Features.Orders.Commands.UpdateOrderStatus;
using HomeTaste.Application.Features.Orders.Queries.GetAllOrders;
using HomeTaste.Application.Features.Orders.Queries.GetDeliveryFee;
using HomeTaste.Application.Features.Orders.Queries.GetMyOrders;
using HomeTaste.Application.Features.Orders.Queries.GetOrderById;
using HomeTaste.Application.Interfaces.Order;
using HomeTaste.Domain.Enums;
using MediatR;
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
        private readonly IMediator _mediator;
        private readonly IPdfInvoiceService _pdfInvoiceService;

        public OrderController(IMediator mediator, IPdfInvoiceService pdfInvoiceService)
        {
            _mediator = mediator;
            _pdfInvoiceService = pdfInvoiceService;
        }

        /// <summary>Returns the delivery fee for a given subtotal before placing an order.</summary>
        [HttpGet("delivery-fee")]
        public async Task<IActionResult> GetDeliveryFee([FromQuery] decimal subTotal)
        {
            var result = await _mediator.Send(new GetDeliveryFeeQuery(subTotal));
            return Ok(result);
        }

        /// <summary>Gets the authenticated customer's orders with pagination.</summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetMyOrdersQuery(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>Gets a single order by ID. Customers see only their own orders.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            return Ok(result);
        }

        /// <summary>Gets all orders with optional status filter and pagination. Admin only.</summary>
        [HttpGet]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null)
        {
            var result = await _mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize, status));
            return Ok(result);
        }

        /// <summary>Places a new order for the authenticated customer.</summary>
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            var items = (request.Items ?? []).Select(i => new PlaceOrderItemCommand(
                i.MealId, i.Quantity, i.SpecialInstructions, i.CustomizationOptionIds)).ToList();

            var result = await _mediator.Send(new PlaceOrderCommand(
                request.AddressId, items, request.CouponCode, request.PointsToRedeem, request.Notes));
            return Ok(result);
        }

        /// <summary>Advances an order through its status workflow. Admin only.</summary>
        [HttpPatch("{id:guid}/status")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _mediator.Send(new UpdateOrderStatusCommand(id, request.Status, request.CancellationReason));
            return Ok(result);
        }

        /// <summary>Cancels an order. Customers can cancel Pending/Confirmed orders; Admins can cancel any.</summary>
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest request)
        {
            var result = await _mediator.Send(new CancelOrderCommand(id, request.Reason));
            return Ok(result);
        }

        /// <summary>Downloads a PDF invoice for the given order.</summary>
        [HttpGet("{id:guid}/invoice")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            var pdf = _pdfInvoiceService.Generate(result.Data!);
            var fileName = $"invoice-{id.ToString()[..8].ToUpper()}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
