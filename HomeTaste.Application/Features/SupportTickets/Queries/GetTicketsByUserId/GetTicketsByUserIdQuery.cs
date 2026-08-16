using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketsByUserId
{
    public record GetTicketsByUserIdQuery(Guid UserId) : IRequest<Result<IEnumerable<SupportTicketResponse>>>;
}
