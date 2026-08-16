using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.Inventory.Commands.AddInventoryItem;
using HomeTaste.Application.Features.Inventory.Commands.BulkInsertInventoryItems;
using HomeTaste.Application.Features.Inventory.Commands.DeleteInventoryItem;
using HomeTaste.Application.Features.Inventory.Commands.UpdateInventoryItem;
using HomeTaste.Application.Features.Inventory.Queries.GetAllInventoryItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get a list of inventory items
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems([FromQuery] int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null!)
        {
            var result = await _mediator.Send(new GetAllInventoryItemsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm });
            return Ok(result);
        }

        // Add a new item to the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost]
        public async Task<IActionResult> AddInventoryItem([FromBody] AddInventoryItemRequest request)
        {
            var result = await _mediator.Send(new AddInventoryItemCommand(request.Name, request.StockCount, request.Price));
            return Ok(result);
        }

        // Update an existing item in the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] UpdateInventoryItemRequest request)
        {
            var result = await _mediator.Send(new UpdateInventoryItemCommand(id, request.StockCount, request.Price));
            return Ok(result);
        }

        // Delete an item from the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(Guid id)
        {
            var result = await _mediator.Send(new DeleteInventoryItemCommand(id));
            return Ok(result);
        }

        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertInventoryItems()
        {
            var result = await _mediator.Send(new BulkInsertInventoryItemsCommand());
            return Ok(result);
        }
    }
}
