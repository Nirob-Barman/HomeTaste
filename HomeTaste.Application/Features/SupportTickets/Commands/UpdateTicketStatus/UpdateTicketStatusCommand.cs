using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Commands.UpdateTicketStatus
{
    public class UpdateTicketStatusCommand : IRequest<Result<string>>
    {
        public Guid TicketId { get; set; }
        public UpdateTicketRequest Request { get; set; }

        public UpdateTicketStatusCommand(Guid ticketId, UpdateTicketRequest request)
        {
            TicketId = ticketId;
            Request = request;
        }
    }
}
