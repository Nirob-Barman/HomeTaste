using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Support
{
    public class UpdateTicketRequest
    {
        public TicketStatus Status { get; set; }  // E.g., "Open", "Resolved"
    }
}
