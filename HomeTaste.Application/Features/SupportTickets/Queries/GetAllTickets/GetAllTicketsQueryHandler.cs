using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.SupportTickets.Queries.GetAllTickets
{
    public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, Result<IEnumerable<SupportTicketResponse>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllTicketsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<IEnumerable<SupportTicketResponse>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.SupportTickets.ToListAsync(cancellationToken);

            var response = tickets.Select(SupportTicketMapper.ToResponse).ToList();
            return Result<IEnumerable<SupportTicketResponse>>.Ok(response, "Tickets retrieved successfully");
        }
    }
}
