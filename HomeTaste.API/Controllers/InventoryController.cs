using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.MealManagement.Inventory;
using HomeTaste.Application.Interfaces.MealManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Get a list of inventory items
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetInventoryItems([FromQuery] int pageNumber = 1,
            int pageSize = 10,
            string searchTerm = null!)
        {
            var result = await _inventoryService.GetAllInventoryItemsAsync(pageNumber, pageSize, searchTerm);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Add a new item to the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost]
        public async Task<IActionResult> AddInventoryItem([FromBody] AddInventoryItemRequest request)
        {
            var result = await _inventoryService.AddInventoryItemAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update an existing item in the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, [FromBody] UpdateInventoryItemRequest request)
        {
            var result = await _inventoryService.UpdateInventoryItemAsync(id, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Delete an item from the inventory
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(Guid id)
        {
            var result = await _inventoryService.DeleteInventoryItemAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        [Authorize(Policy = Policies.AdminOnly)]
        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertInventoryItems()
        {
            // Call the service method to insert predefined inventory items
            var result = await _inventoryService.BulkInsertInventoryItemsAsync();

            // Map the result to an API response format (similar to how you handled predefined units)
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
