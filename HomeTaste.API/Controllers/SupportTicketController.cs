using HomeTaste.API.Wrappers;
using HomeTaste.Application.Authorization;
using HomeTaste.Application.DTOs.Support;
using HomeTaste.Application.Interfaces.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly ISupportTicketService _supportTicketService;

        public SupportTicketController(ISupportTicketService supportTicketService)
        {
            _supportTicketService = supportTicketService;
        }

        // Get all support tickets (for admin)
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var result = await _supportTicketService.GetAllTicketsAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        // Create a new support ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            var result = await _supportTicketService.CreateTicketAsync(request);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get a specific support ticket by ID
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(Guid ticketId)
        {
            var result = await _supportTicketService.GetTicketByIdAsync(ticketId);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Update the status of a support ticket
        [HttpPatch("{ticketId}")]
        public async Task<IActionResult> UpdateTicketStatus(Guid ticketId, [FromBody] UpdateTicketRequest request)
        {
            var result = await _supportTicketService.UpdateTicketStatusAsync(ticketId, request);
            return ApiResponseMapper.FromResult(this, result);
        }

        // Get all support tickets for a specific user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUserId(Guid userId)
        {
            var result = await _supportTicketService.GetTicketsByUserIdAsync(userId);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
