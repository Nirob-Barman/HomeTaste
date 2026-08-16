using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketById
{
    public record GetTicketByIdQuery(Guid TicketId) : IRequest<Result<SupportTicketResponse>>;
}
