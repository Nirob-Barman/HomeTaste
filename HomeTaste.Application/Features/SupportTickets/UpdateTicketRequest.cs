using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.SupportTickets
{
    public record UpdateTicketRequest
    {
        public TicketStatus Status { get; set; }
    }
}
