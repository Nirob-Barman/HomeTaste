using HomeTaste.Application.Wrappers;
using MediatR;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketsByUserId
{
    public class GetTicketsByUserIdQuery : IRequest<Result<IEnumerable<SupportTicketResponse>>>
    {
        public Guid UserId { get; set; }

        public GetTicketsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
