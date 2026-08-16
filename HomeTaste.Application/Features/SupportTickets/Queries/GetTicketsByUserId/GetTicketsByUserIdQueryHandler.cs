using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetTicketsByUserId
{
    public class GetTicketsByUserIdQueryHandler : IRequestHandler<GetTicketsByUserIdQuery, Result<IEnumerable<SupportTicketResponse>>>
    {
        private readonly IApplicationDbContext _context;

        public GetTicketsByUserIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<SupportTicketResponse>>> Handle(GetTicketsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.SupportTickets
                .Where(t => t.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            var response = tickets.Select(SupportTicketMapper.ToResponse).ToList();
            return Result<IEnumerable<SupportTicketResponse>>.Ok(response, "Tickets retrieved successfully");
        }
    }
}
