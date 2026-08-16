using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetAllTickets
{
    public record GetAllTicketsQuery : IRequest<Result<IEnumerable<SupportTicketResponse>>>;
}
