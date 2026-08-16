using HomeTaste.Application.Authorization;
using HomeTaste.Application.Features.SupportTickets;
using HomeTaste.Application.Features.SupportTickets.Commands.CreateTicket;
using HomeTaste.Application.Features.SupportTickets.Commands.UpdateTicketStatus;
using HomeTaste.Application.Features.SupportTickets.Queries.GetAllTickets;
using HomeTaste.Application.Features.SupportTickets.Queries.GetTicketById;
using HomeTaste.Application.Features.SupportTickets.Queries.GetTicketsByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupportTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Get all support tickets (for admin)
        [Authorize(Policy = Policies.AdminOnly)]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            var result = await _mediator.Send(new GetAllTicketsQuery());
            return Ok(result);
        }

        // Create a new support ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
        {
            var result = await _mediator.Send(new CreateTicketCommand(request));
            return Ok(result);
        }

        // Get a specific support ticket by ID
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(Guid ticketId)
        {
            var result = await _mediator.Send(new GetTicketByIdQuery(ticketId));
            return Ok(result);
        }

        // Update the status of a support ticket
        [HttpPatch("{ticketId}")]
        public async Task<IActionResult> UpdateTicketStatus(Guid ticketId, [FromBody] UpdateTicketRequest request)
        {
            var result = await _mediator.Send(new UpdateTicketStatusCommand(ticketId, request));
            return Ok(result);
        }

        // Get all support tickets for a specific user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTicketsByUserId(Guid userId)
        {
            var result = await _mediator.Send(new GetTicketsByUserIdQuery(userId));
            return Ok(result);
        }
    }
}
