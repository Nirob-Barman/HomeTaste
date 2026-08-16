using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketById
{
    public class GetTicketByIdQuery : IRequest<Result<SupportTicketResponse>>
    {
        public Guid TicketId { get; set; }

        public GetTicketByIdQuery(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
}
