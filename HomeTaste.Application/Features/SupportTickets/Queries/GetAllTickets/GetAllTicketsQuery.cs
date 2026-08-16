using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetAllTickets
{
    public class GetAllTicketsQuery : IRequest<Result<IEnumerable<SupportTicketResponse>>>
    {
    }
}
